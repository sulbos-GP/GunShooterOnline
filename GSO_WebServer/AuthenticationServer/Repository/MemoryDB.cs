using AuthenticationServer.Models;
using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Options;

namespace AuthenticationServer.Repository
{
    public class MemoryDB : IMemoryDB
    {
        public RedisConnection? mRedisConn;
        public MemoryDB(IOptions<DbConfig> dbConfig)
        {
            if(dbConfig.Value.Redis != null)
            {
                RedisConfig config = new("default", dbConfig.Value.Redis);
                mRedisConn = new RedisConnection(config);
            }
        }

        public void Dispose()
        {
        }


        public async Task<ErrorCode> RegistUserAsync(long uid, long expires, string accessToken, string refreshToken)
        {

            string key = uid.ToString();
            ErrorCode result = ErrorCode.None;

            RegisterUserData user = new RegisterUserData
            {
                uid = key,
                access_token = accessToken,
                refresh_token = refreshToken
            };

            try
            {
                if (mRedisConn != null)
                {
                    var expiryTime = TimeSpan.FromSeconds(expires);
                    RedisString<RegisterUserData> redis = new(mRedisConn, key, expiryTime);
                    if (await redis.SetAsync(user, expiryTime) == false)
                    {
                        result = ErrorCode.LoginFailAddRedis;
                        return result;
                    }
                }
            }
            catch
            {
                result = ErrorCode.LoginFailAddRedis;
                return result;
            }

            return result;
        }
    }
}
