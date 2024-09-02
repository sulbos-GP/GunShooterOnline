using CloudStructures.Structures;
using CloudStructures;
using Docker.DotNet;
using GameServerManager.Repository.Interfaces;
using GSO_WebServerLibrary.Config;
using Microsoft.Extensions.Options;
using GameServerManager.Models;
using GSO_WebServerLibrary.Error;
using GSO_WebServerLibrary.Utils;
using StackExchange.Redis;

namespace GameServerManager.Repository
{
    public class SessionMemory : ISessionMemory
    {
        public RedisConnection? mRedisConn;
        private RedisDictionary<string, MatchStatus> mMatchStatus;

        public SessionMemory(IOptions<DatabaseConfig> dbConfig)
        {
            if (dbConfig.Value.Redis != null)
            {
                RedisConfig config = new("SessionQueue", dbConfig.Value.Redis);
                mRedisConn = new RedisConnection(config);
                mMatchStatus = new RedisDictionary<string, MatchStatus>(mRedisConn, "MatchStatus", null);
            }
        }

        public async Task<WebErrorCode> AddMatchStatus(String containerId, String name, String world, EMatchState state, String hostIp, Int32 containerPort, Int32 hostPort, DateTime age)
        {

            MatchStatus status = new MatchStatus
            {
                name = name,
                world = world,
                state = state,
                host_ip = hostIp,
                container_port = containerPort,
                host_port = hostPort,
                retries = 0,
                age = new DateTimeOffset(age).ToUnixTimeSeconds()
            };

            bool reuslt = await mMatchStatus.SetAsync(containerId, status);
            if (reuslt == false)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            return WebErrorCode.None;
        }

        public async Task<WebErrorCode> RemoveMatchStatus(String containerId)
        {

            bool reuslt = await mMatchStatus.DeleteAsync(containerId);
            if (reuslt == false)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            return WebErrorCode.None;
        }

        public async Task<bool> UpdateMatchStatus(String containerId, MatchStatus status)
        {
            return await mMatchStatus.SetAsync(containerId, status);
        }

        public async Task<Dictionary<String, MatchStatus>?> GetAllMatchStatus()
        {
            return await mMatchStatus.GetAllAsync();
        }

        public async Task<MatchStatus?> GetMatchStatus(String containerId)
        {
            var result = await mMatchStatus.GetAsync(containerId);
            if (result.HasValue)
            {
                return result.Value;
            }

            return null;
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
