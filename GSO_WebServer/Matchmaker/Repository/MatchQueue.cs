using Matchmaker.Models;
using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Options;
using Matchmaker.Repository.Interface;
using GSO_WebServerLibrary.Config;
using GSO_WebServerLibrary.Error;
using GSO_WebServerLibrary.Utils;
using StackExchange.Redis;

namespace Matchmaker.Repository
{
    public class MatchQueue : IMatchQueue
    {
        public RedisConnection? mRedisConn;
        private RedisSortedSet<string> mMatchRating;                    //플레이어 레이팅 나열
        private RedisDictionary<string, TicketInfo> mMatchTickets;      //플레이어 정보(티켓)

        public MatchQueue(IOptions<DatabaseConfig> envConfig)
        {
            if(envConfig.Value.Redis != null)
            {
                RedisConfig config = new("MatchQueue", envConfig.Value.Redis);
                mRedisConn = new RedisConnection(config);
                mMatchRating = new RedisSortedSet<string>(mRedisConn, "Ratings", null);
                mMatchTickets = new RedisDictionary<string, TicketInfo>(mRedisConn, "Tickets", null);
            }
        }

        public void Dispose()
        {
            if (mRedisConn != null)
            {
                mRedisConn.GetConnection().Close();
            }
        }

        public async Task<WebErrorCode> AddMatchRating(Int32 uid, Double rating)
        {

            string key = KeyUtils.MakeKey(KeyUtils.EKey.MATCH, uid);

            var reuslt = await mMatchRating.AddAsync(key, rating, null, When.NotExists);
            if (reuslt == false)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            return WebErrorCode.None;
        }

        public async Task<WebErrorCode> RemoveMatchRating(Int32 uid)
        {
            string key = KeyUtils.MakeKey(KeyUtils.EKey.MATCH, uid);

            bool result = await mMatchRating.RemoveAsync(key);
            if (result == false)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            return WebErrorCode.None;
        }

        public async Task<RedisSortedSetEntry<string>[]?> GetAllMatchRating()
        {
            return await mMatchRating.RangeByRankWithScoresAsync();
        }

        public async Task<string[]?> SearchPlayerByRange(Double min, Double max)
        {
            return await mMatchRating.RangeByScoreAsync(min, max);
        }

        public async Task<WebErrorCode> AddMatchTicket(Int32 uid, String clientId)
        {

            string key = KeyUtils.MakeKey(KeyUtils.EKey.MATCH, uid);

            TicketInfo ticket = new TicketInfo
            {
                client_id = clientId,
            };

            bool reuslt = await mMatchTickets.SetAsync(key, ticket);
            if (reuslt == false)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            return WebErrorCode.None;

        }

        public async Task<WebErrorCode> RemoveMatchTicket(Int32 uid)
        {
            string key = KeyUtils.MakeKey(KeyUtils.EKey.MATCH, uid);

            bool reuslt = await mMatchTickets.DeleteAsync(key);
            if (reuslt == false)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            return WebErrorCode.None;
        }

        public async Task<Dictionary<string, TicketInfo>?> GetAllMatchTicket()
        {
            return await mMatchTickets.GetAllAsync();
        }

        public async Task<TicketInfo?> GetPlayerTicket(Int32 uid)
        {

            string key = KeyUtils.MakeKey(KeyUtils.EKey.MATCH, uid);

            var result = await mMatchTickets.GetAsync(key);
            if(result.HasValue)
            {
                return result.Value;
            }

            return null;
        }

        public async Task<Dictionary<string, TicketInfo>?> GetPlayerTickets(string[] keys)
        {
            return await mMatchTickets.GetAsync(keys);
        }

        public async Task<bool> UpdateTicket(Int32 uid, TicketInfo ticket)
        {
            string key = KeyUtils.MakeKey(KeyUtils.EKey.MATCH, uid);
            return await mMatchTickets.SetAsync(key, ticket);
        }

    }
}
