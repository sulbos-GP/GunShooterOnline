using Google.Apis.Auth.OAuth2;
using Google.Apis.Games.v1.Data;
using Google.Apis.Games.v1;
using Google.Apis.Services;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using GSO_WebServerLibrary.Servicies.Interfaces;
using GsoWebServer.Servicies.Interfaces;
using Google.Apis.Auth.OAuth2.Responses;
using GSO_WebServerLibrary.Error;

namespace GsoWebServer.Servicies.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IGoogleService mGoogleService;
        private readonly IGameDB mGameDB;
        private readonly IMemoryDB mSharedDB;
        public AuthenticationService(IGoogleService google, IGameDB gameDB, IMemoryDB sharedDB)
        {
            mGoogleService = google;
            mGameDB = gameDB;
            mSharedDB = sharedDB;
        }

        public void Dispose()
        {

        }

        public async Task<(WebErrorCode, TokenResponse?)> ExchangeToken(String userId, String serverCode)
        {
            return await mGoogleService.ExchangeToken(userId, serverCode);
        }

        public async Task<(WebErrorCode, TokenResponse?)> RefreshToken(String userId, String refreshToken)
        {
            return await mGoogleService.RefreshToken(userId, refreshToken);
        }

        public async Task<WebErrorCode> RevokeToken(String userId, String accessToken)
        {
            return await mGoogleService.RevokeToken(userId, accessToken);
        }

        public async Task<(WebErrorCode, Player?)> GetMyPlayer(String userId, String accessToken)
        {
            return await mGoogleService.GetMyPlayer(userId, accessToken);
        }

        public async Task<(WebErrorCode, int)> VerifyUser(String playerId, String service)
        {
            try
            {

                var userInfo = await mGameDB.GetUserByPlayerId(playerId);

                if (userInfo is null)
                {
                    return (WebErrorCode.SignInFailUserNotExist, 0);
                }

                if (userInfo.service != service)
                {
                    return (WebErrorCode.SignInFailMismatchService, 0);
                }

                return (WebErrorCode.None, userInfo.uid);
            }
            catch /*(Exception ex)*/
            {
                return (WebErrorCode.MyPlayerFailException, 0);
            }
        }

        public async Task<WebErrorCode> VerifyNickname(Int32 uid, String nickname)
        {
            try
            {

                var userInfo = await mGameDB.GetUserByUid(uid);

                if (userInfo is null)
                {
                    return (WebErrorCode.SignInFailUserNotExist);
                }

                if (userInfo.nickname == string.Empty)
                {
                    return (WebErrorCode.None);
                }

                if(userInfo.nickname == nickname)
                {
                    return (WebErrorCode.SetNicknameFailSameNickname);
                }

                return (WebErrorCode.None);
            }
            catch /*(Exception ex)*/
            {
                return (WebErrorCode.MyPlayerFailException);
            }
        }

        public async Task<WebErrorCode> RegisterToken(Int32 uid, String user_id, String accessToken, String refreshToken, Int64 expires)
        {

            var error = await mSharedDB.RegisterAuthUserData(uid, user_id, accessToken, expires);
            if (error != WebErrorCode.None)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            //error = await mSharedDB.RegisterRefreshToken(uid, user_id, refreshToken);
            //if (error != WebErrorCode.None)
            //{
            //    return WebErrorCode.TEMP_ERROR;
            //}

            return WebErrorCode.None;

        }

        public async Task<WebErrorCode> RemoveToken(Int32 uid)
        {
            var error = await mSharedDB.RemoveAuthUserData(uid);
            if (error != WebErrorCode.None)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            error = await mSharedDB.RemoveRefreshToken(uid);
            if (error != WebErrorCode.None)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            return WebErrorCode.None;
        }

        public async Task<WebErrorCode> UpdateLastSignInTime(int uid)
        {
            try
            {
                var rowCount = await mGameDB.UpdateRecentLogin(uid);

                if (rowCount != 1)
                {
                    return WebErrorCode.AccountIdMismatch;
                }

                return WebErrorCode.None;
            }
            catch /*(Exception e)*/
            { 
                return WebErrorCode.AccountIdMismatch;
            }
        }
    }
}
