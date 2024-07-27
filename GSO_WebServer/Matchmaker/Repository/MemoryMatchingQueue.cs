using Matchmaker.Models;
using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Options;
using GSO_WebServerLibrary;
using Microsoft.ML.Probabilistic.Factors;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ZLogger;
using System.Linq;
using Microsoft.VisualBasic;

namespace Matchmaker.Repository
{
    public class MemoryMatchingQueue : IMatchingQueue
    {
        public RedisConnection? mRedisConn;
        private RedisSortedSet<string> mMatchingQueue;
        private RedisDictionary<string, MatchQueueInfo> mMatchingQueueInfos;

        public MemoryMatchingQueue(IOptions<DbConfig> dbConfig)
        {
            if(dbConfig.Value.Redis != null)
            {
                RedisConfig config = new("Matching", dbConfig.Value.Redis);
                mRedisConn = new RedisConnection(config);
                mMatchingQueue = new RedisSortedSet<string>(mRedisConn, "MatchingQueue", null);
                mMatchingQueueInfos = new RedisDictionary<string, MatchQueueInfo>(mRedisConn, "MatchingPlayerInfo", null);
            }
        }

        public async Task<WebErrorCode> PushAsync(long uid, double rating, double deviation)
        {
            string key = "MATCH_" + uid.ToString();

            MatchQueueInfo user = new MatchQueueInfo
            {
                uid = key,
                rating = rating,
                deviation = deviation,
                state = "Waiting",
                count = 0,
                time = DateTime.UtcNow.Ticks
            };

            try
            {
                if (mRedisConn != null)
                {

                    var reuslt = await mMatchingQueue.AddAsync(key, rating, null, StackExchange.Redis.When.NotExists);
                    if(reuslt == false)
                    {

                    }

                    reuslt = await mMatchingQueueInfos.SetAsync(key, user);
                    if (reuslt == false)
                    {
                        return WebErrorCode.PushPlayerSkillIsExist;
                    }


                    return WebErrorCode.None;

                }
            }
            catch
            {
                return WebErrorCode.PushPlayerSkillFailException;
            }

            return WebErrorCode.GetRedisConnectionFail;
        }

        public async Task<WebErrorCode> PopAsync(long uid)
        {
            string key = "MATCH_" + uid.ToString();

            try
            {
                if (mRedisConn != null)
                {
                    bool result = await mMatchingQueue.RemoveAsync(key);
                    if (result == false)
                    {
                        return WebErrorCode.None;
                    }

                    result = await mMatchingQueueInfos.DeleteAsync(key);
                    if (result == false)
                    {
                        return WebErrorCode.None;
                    }
                }
            }
            catch
            {
                return WebErrorCode.None;
            }

            return WebErrorCode.None;
        }

        /// <summary>
        ///  매칭에 있는 모든 플레이어들 조회
        /// </summary>
        /// <returns></returns>
        public async Task<Tuple<WebErrorCode, List<MatchQueueInfo>?>> ScanPlayers()
        {
            try
            {
                if (mRedisConn != null)
                {
                    var players = await mMatchingQueue.RangeByRankAsync();
                    var infos = await mMatchingQueueInfos.GetAllAsync();

                    List<MatchQueueInfo> matcheInfos = new List<MatchQueueInfo>();
                    foreach (var player in players)
                    {
                        MatchQueueInfo? info;
                        if(infos.TryGetValue(player, out info))
                        {
                            matcheInfos.Add(info);
                        }
                        else
                        {
                            await mMatchingQueue.RemoveAsync(player);
                            await mMatchingQueueInfos.DeleteAsync(player);
                        }
                    }

                    List<MatchQueueInfo> sortedMatchQueueInfos = matcheInfos.OrderBy(info => info.time).ToList();

                    return new Tuple<WebErrorCode, List<MatchQueueInfo>?>(WebErrorCode.None, sortedMatchQueueInfos);
                }
            }
            catch
            {
                return new Tuple<WebErrorCode, List<MatchQueueInfo>?>(WebErrorCode.ScanPlayersToFailException, null);
            }

            return new Tuple<WebErrorCode, List<MatchQueueInfo>?>(WebErrorCode.GetRedisConnectionFail, null);
        }

        /// <summary>
        /// 레이팅에 따른 플레이어 매칭
        /// </summary>
        public async Task<Tuple<WebErrorCode, MatchQueueInfo[]?>> FindMatch(double minRating, double maxRating, int number)
        {

            try
            {
                if (mRedisConn != null)
                {

                    var uids = await mMatchingQueue.RangeByScoreAsync(minRating, maxRating);

                    if (uids.Length < number)
                    {
                        return new Tuple<WebErrorCode, MatchQueueInfo[]?>(WebErrorCode.FindMatchFailFindPossibleOpponents, null);
                    }

                    List<MatchQueueInfo> infos = new List<MatchQueueInfo>();
                    foreach (var uid in uids)
                    {
                        var info = await mMatchingQueueInfos.GetAsync(uid);
                        if(info.HasValue)
                        {
                            infos.Add(info.Value);
                        }
                    }

                    List<MatchQueueInfo> sortedMatchQueueInfos = infos.OrderBy(info => info.time).Take(number).ToList();


                    return new Tuple<WebErrorCode, MatchQueueInfo[]?>(WebErrorCode.None, sortedMatchQueueInfos.ToArray());
                }
            }
            catch
            {
                return new Tuple<WebErrorCode, MatchQueueInfo[]?>(WebErrorCode.FindMatchToFailException, null);
            }

            return new Tuple<WebErrorCode, MatchQueueInfo[]?>(WebErrorCode.GetRedisConnectionFail, null);
        }

        /// <summary>
        /// 플레이어들 레디스에서 삭제
        /// </summary>
        public async Task<Tuple<WebErrorCode, bool>> RemovePlayers(MatchQueueInfo[]? infos)
        {

            try
            {
                if (mRedisConn != null)
                {

                    if(infos == null)
                    {
                        return new Tuple<WebErrorCode, bool>(WebErrorCode.MatchmakingFailToRemovePlayer, false);
                    }

                    foreach (var info in infos)
                    {
                        var removeResult = await mMatchingQueue.RemoveAsync(info.uid);
                        if(removeResult == false)
                        {
                            return new Tuple<WebErrorCode, bool>(WebErrorCode.MatchmakingFailToRemovePlayer, false);
                        }

                        var deleteResult = await mMatchingQueueInfos.DeleteAsync(info.uid);
                        if (deleteResult == false)
                        {
                            return new Tuple<WebErrorCode, bool>(WebErrorCode.MatchmakingFailToRemovePlayer, false);
                        }

                    }

                    return new Tuple<WebErrorCode, bool>(WebErrorCode.None, true);
                }
            }
            catch
            {
                return new Tuple<WebErrorCode, bool>(WebErrorCode.RemoveMatchPlayersFailException, false);
            }

            return new Tuple<WebErrorCode, bool>(WebErrorCode.GetRedisConnectionFail, false);
        }

        public async Task<Tuple<WebErrorCode, bool>> UpdatePlayer(MatchQueueInfo newInfo)
        {
            try
            {
                if (mRedisConn != null)
                {
                    bool result = await mMatchingQueueInfos.SetAsync(newInfo.uid, newInfo, null, StackExchange.Redis.When.Always);
                    if (result == true)
                    {
                        return new Tuple<WebErrorCode, bool>(WebErrorCode.UpdatePlayerToFailException, false);
                    }

                    return new Tuple<WebErrorCode, bool>(WebErrorCode.None, true);
                }
            }
            catch
            {
                return new Tuple<WebErrorCode, bool>(WebErrorCode.UpdatePlayerToFailException, false);
            }

            return new Tuple<WebErrorCode, bool>(WebErrorCode.GetRedisConnectionFail, false);
        }

        public void Dispose()
        {
        }

    }
}
