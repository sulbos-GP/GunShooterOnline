using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Options;
using Matchmaker.Repository.Interface;
using GSO_WebServerLibrary.Utils;
using StackExchange.Redis;
using WebCommonLibrary.Models.Match;
using WebCommonLibrary.Config;
using WebCommonLibrary.Error;
using static Pipelines.Sockets.Unofficial.Threading.MutexSlim;
using System.Diagnostics;

namespace Matchmaker.Repository
{

    public class RedisEventLock
    {
        private RedisLock<string> _lock;
        private ISubscriber sub;
        private RedisChannel channle;

        private readonly string key;

        public RedisEventLock(RedisConnection connection, int uid)
        {
            this.key = KeyUtils.MakeKey(KeyUtils.EKey.MATCHLock, uid);

            _lock = new RedisLock<string>(connection, key);
            sub = connection.GetConnection().GetSubscriber();

            channle = new RedisChannel("lock-released", RedisChannel.PatternMode.Literal);
        }

        public async Task<bool> TryLock(int uid, bool isWaiting = true)
        {
            bool result = false;
            do
            {
                string token = KeyUtils.MakeKey(KeyUtils.EKey.MATCHLock, uid);
                bool isLocked = await _lock.TakeAsync(token, TimeSpan.FromSeconds(30));
                if (isLocked)
                {
                    result = true; 
                    break;
                }
                else
                {
                    if (isWaiting)
                    {
                        await sub.SubscribeAsync(channle, (channel, message) => { });
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }

            }while (true);

            return result;
        }

        public async Task ReleaseLock(int uid)
        {
            string token = KeyUtils.MakeKey(KeyUtils.EKey.MATCHLock, uid);
            bool isRelease = await _lock.ReleaseAsync(token);
            if (isRelease)
            {
                await sub.PublishAsync(channle, "Lock has been released");
            }
            else
            {
                Console.WriteLine("락 해제에 실패하였습니다.");
            }
        }
    };


    public partial class MatchQueue : IMatchQueue
    {
        public RedisConnection mRedisConn;
        private RedisSortedSet<string> mMatchRating;                //플레이어 레이팅 나열
        private RedisDictionary<string, Ticket> mMatchTickets;      //플레이어 정보(티켓)
        private Dictionary<int, RedisEventLock> eventLocks;         //플레이어 락

        public MatchQueue(IOptions<DatabaseConfig> envConfig)
        {
            RedisConfig config = new("MatchQueue", envConfig.Value.Redis);
            mRedisConn = new RedisConnection(config);
            mMatchRating = new RedisSortedSet<string>(mRedisConn, "Ratings", null);
            mMatchTickets = new RedisDictionary<string, Ticket>(mRedisConn, "Tickets", null);
            eventLocks = new Dictionary<int, RedisEventLock>();
        }

        public async Task<bool> TryTakeLock(int uid, bool isWaiting)
        {
            eventLocks.TryGetValue(uid, out var eventLock);
            if (eventLock == null)
            {
                RedisEventLock newEventLock = new RedisEventLock(mRedisConn, uid);
                eventLocks.Add(uid, newEventLock);
                return await newEventLock.TryLock(uid);
            }

            return await eventLock.TryLock(uid);
        }

        public async Task ReleaseLock(int uid)
        {
            eventLocks.TryGetValue(uid, out var eventLock);
            if (eventLock == null)
            {
                return;
            }

            await eventLock.ReleaseLock(uid);
        }       

        public void RemoveLock(int uid)
        {
            eventLocks.Remove(uid);
        }
        
        public void Dispose()
        {
            if (mRedisConn != null)
            {
                mRedisConn.GetConnection().Close();
            }
        }

    }
}
