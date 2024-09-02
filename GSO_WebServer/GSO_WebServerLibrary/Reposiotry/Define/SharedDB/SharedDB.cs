using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Options;
using WebCommonLibrary.Models.MemoryDB;
using WebCommonLibrary.Error;
using GSO_WebServerLibrary.Utils;
using GSO_WebServerLibrary.Reposiotry.Interfaces;

namespace GSO_WebServerLibrary.Reposiotry.Define.MemoryDB
{
    public partial class MemoryDB : IMemoryDB
    {

        public async Task<WebErrorCode> RegisterAuthUserData(Int32 uid, String userID, String accessToken, Int64 expires)
        {

            string key = KeyUtils.MakeKey(KeyUtils.EKey.UID, uid);

            AuthUserDataInfo user = new AuthUserDataInfo
            {
                uid = key,
                user_id = userID,
                access_token = accessToken,
            };

            try
            {
                var expiryTime = TimeSpan.FromSeconds(expires);
                RedisString<AuthUserDataInfo> redis = new(mRedisConn, key, expiryTime);
                if (await redis.SetAsync(user, expiryTime) == false)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                return WebErrorCode.None;
            }
            catch
            {
                return WebErrorCode.TEMP_Exception;
            }

        }

        public async Task<WebErrorCode> RemoveAuthUserData(Int32 uid)
        {
            string key = KeyUtils.MakeKey(KeyUtils.EKey.UID, uid);

            try
            {
                RedisString<AuthUserDataInfo> redis = new(mRedisConn, key, null);
                if (await redis.DeleteAsync() == false)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                return WebErrorCode.None;
            }
            catch
            {
                return WebErrorCode.TEMP_Exception;
            }
        }

        public async Task<WebErrorCode> RegisterRefreshToken(Int32 uid, String userID, String refreshToken)
        {

            string key = KeyUtils.MakeKey(KeyUtils.EKey.REFRESH, uid);

            RefreshDataInfo user = new RefreshDataInfo
            {
                uid = key,
                user_id = userID,
                refresh_token = refreshToken,
            };

            try
            {
                RedisString<RefreshDataInfo> redis = new(mRedisConn, key, null);
                if (await redis.SetAsync(user, null) == false)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                return WebErrorCode.None;
            }
            catch
            {
                return WebErrorCode.TEMP_Exception;
            }

        }

        public async Task<WebErrorCode> RemoveRefreshToken(Int32 uid)
        {
            string key = KeyUtils.MakeKey(KeyUtils.EKey.REFRESH, uid);

            try
            {
                RedisString<RefreshDataInfo> redis = new(mRedisConn, key, null);
                if (await redis.DeleteAsync() == false)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                return WebErrorCode.None;
            }
            catch
            {
                return WebErrorCode.TEMP_Exception;
            }
        }

        public async Task<(WebErrorCode, AuthUserDataInfo?)> ValidateAndGetUserData(Int32 uid)
        {
            try
            {

                string key = KeyUtils.MakeKey(KeyUtils.EKey.UID, Convert.ToInt32(uid));

                RedisString<AuthUserDataInfo> redis = new(mRedisConn, key, null);
                var user = await redis.GetWithExpiryAsync();
                if (user.HasValue == false || user.Expiry.HasValue == false)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                return (WebErrorCode.None, user.Value);
            }
            catch
            {
                return (WebErrorCode.TEMP_Exception, null);
            }
        }

        public async Task<(WebErrorCode, RefreshDataInfo?)> ValidateAndGetRefreshToken(Int32 uid)
        {
            try
            {

                string key = KeyUtils.MakeKey(KeyUtils.EKey.REFRESH, Convert.ToInt32(uid));

                RedisString<RefreshDataInfo> redis = new(mRedisConn, key, null);
                var refreshToken = await redis.GetAsync();
                if (refreshToken.HasValue == false)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                return (WebErrorCode.None, refreshToken.Value);
            }
            catch
            {
                return (WebErrorCode.TEMP_Exception, null);
            }
        }

        public async Task<WebErrorCode> RegisterLockAuthUserData(Int32 uid)
        {
            try
            {

                string key = KeyUtils.MakeKey(KeyUtils.EKey.UIDLock, Convert.ToInt32(uid));

                AuthUserDataInfo user = new AuthUserDataInfo();

                RedisString<AuthUserDataInfo> redis = new(mRedisConn, key, null);
                if (await redis.SetAsync(user, null, StackExchange.Redis.When.NotExists) == false) 
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                return WebErrorCode.None;

            }
            catch
            {
                return WebErrorCode.TEMP_Exception;
            }
        }

        public async Task<WebErrorCode> RemoveLockAuthUserData(Int32 uid)
        {
            try
            {
                string key = KeyUtils.MakeKey(KeyUtils.EKey.UIDLock, Convert.ToInt32(uid));

                RedisString<AuthUserDataInfo> redis = new(mRedisConn, key, null);
                if (await redis.DeleteAsync() == false)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                return WebErrorCode.None;

            }
            catch
            {
                return WebErrorCode.TEMP_Exception;
            }
        }
    }
}
