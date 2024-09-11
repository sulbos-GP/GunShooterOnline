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

namespace Matchmaker.Repository
{

    public class RedisEventLock
    {
        private RedisLock<string> _lock;
        private ISubscriber sub;
        private RedisChannel channle;

        private readonly string key;
        private readonly string token;

        public RedisEventLock(RedisConnection connection, string key, string token = "lock")
        {
            this.key = key;
            this.token = token;

            _lock = new RedisLock<string>(connection, key);
            sub = connection.GetConnection().GetSubscriber();

            channle = new RedisChannel("lock-released", RedisChannel.PatternMode.Literal);
        }

        public async void TryLock()
        {
            while (true)
            {
                bool isLocked = await _lock.TakeAsync(token, TimeSpan.FromSeconds(30));
                if (isLocked)
                {
                    break;
                }
                else
                {
                    await sub.SubscribeAsync(channle, (channel, message) => { });
                }
            }
        }

        public async void ReleaseLock()
        {
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
        private RedisEventLock redisEventLock;

        public MatchQueue(IOptions<DatabaseConfig> envConfig)
        {
            RedisConfig config = new("MatchQueue", envConfig.Value.Redis);
            mRedisConn = new RedisConnection(config);
            mMatchRating = new RedisSortedSet<string>(mRedisConn, "Ratings", null);
            mMatchTickets = new RedisDictionary<string, Ticket>(mRedisConn, "Tickets", null);

            redisEventLock = new RedisEventLock(mRedisConn, "MatchQueue", "lock");
        }

        public RedisEventLock GetRedisEventLock()
        {
            return redisEventLock;
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
