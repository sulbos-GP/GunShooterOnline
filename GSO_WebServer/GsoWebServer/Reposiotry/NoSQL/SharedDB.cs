using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Options;
using GsoWebServer.Models.Config;
using GsoWebServer.Reposiotry.Interfaces;
using GSO_WebServerLibrary;
using GsoWebServer.Models.MemoryDB;

namespace GsoWebServer.Reposiotry.NoSQL
{
    public partial class MemoryDB : IMemoryDB
    {

        public async Task<WebErrorCode> RegisterToken(Int32 uid, Int64 expires, String accessToken, String refreshToken)
        {

            string key = KeyUtils.MakeKey(KeyUtils.EKey.UID, uid);

            AuthUserDataInfo user = new AuthUserDataInfo
            {
                uid = key,
                access_token = accessToken,
                refresh_token = refreshToken
            };

            try
            {
                var expiryTime = TimeSpan.FromSeconds(expires);
                RedisString<AuthUserDataInfo> redis = new(mRedisConn, key, expiryTime);
                if (await redis.SetAsync(user, expiryTime) == false)
                {
                    return WebErrorCode.LoginFailAddRedis;
                }

                return WebErrorCode.None;
            }
            catch
            {
                return WebErrorCode.LoginFailAddRedis;
            }

        }
    }
}
