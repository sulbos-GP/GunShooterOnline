using Google.Apis.Auth.OAuth2;
using Google.Apis.Games.v1.Data;
using Google.Apis.Games.v1;
using Google.Apis.Services;
using GSO_WebServerLibrary;
using GsoWebServer.Reposiotry.Interfaces;
using GsoWebServer.Servicies.Interfaces;
using Microsoft.Extensions.Options;
using Google.Apis.Auth.OAuth2.Responses;
using static Google.Apis.Requests.RequestError;

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

        public async Task<(WebErrorCode, TokenResponse?)> RefreshToken(String userId, String serverCode)
        {
            return await mGoogleService.RefreshToken(userId, serverCode);
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

                return (WebErrorCode.MyPlayerFailException, userInfo.uid);
            }
            catch /*(Exception ex)*/
            {
                return (WebErrorCode.MyPlayerFailException, 0);
            }
        }

        public async Task<WebErrorCode> RegisterToken(int uid, long expires, String accessToken, String refreshToken)
        {
            return await mSharedDB.RegisterToken(uid, expires, accessToken, refreshToken);
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
