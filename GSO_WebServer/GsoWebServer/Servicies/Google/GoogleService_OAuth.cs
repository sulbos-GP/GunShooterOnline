using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2;
using GsoWebServer.Servicies.Interfaces;
using static Google.Apis.Requests.BatchRequest;
using GSO_WebServerLibrary;

namespace GsoWebServer.Servicies.Google
{
    public partial class GoogleService : IGoogleService
    {
        public async Task<(WebErrorCode, TokenResponse?)> ExchangeToken(String userId, String serverCode)
        {
            try
            {
                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = mGoogleConfig.Value.ClientId,
                        ClientSecret = mGoogleConfig.Value.ClientSecret,
                    },
                });

                var response = await flow.ExchangeCodeForTokenAsync(userId, serverCode, mGoogleConfig.Value.RedirectUri, CancellationToken.None);

                return (WebErrorCode.None, response);
            }
            catch /*(Exception ex)*/
            {
                return (WebErrorCode.IsNotValidateServerCode, null);
            }
        }

        public async Task<WebErrorCode> RevokeToken(String userId, String accessToken)
        {
            try
            {
                var initializer = new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = mGoogleConfig.Value.ClientId,
                        ClientSecret = mGoogleConfig.Value.ClientSecret,
                    }
                };

                var flow = new GoogleAuthorizationCodeFlow(initializer);
                await flow.RevokeTokenAsync(userId, accessToken, CancellationToken.None);

                return WebErrorCode.None;
            }
            catch /*(Exception ex)*/
            {
                return WebErrorCode.AuthTokenFailSetNx;
            }
        }

        public async Task<(WebErrorCode, TokenResponse?)> RefreshToken(String userId, String refreshToken)
        {

            try
            {
                var initializer = new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = mGoogleConfig.Value.ClientId,
                        ClientSecret = mGoogleConfig.Value.ClientSecret,
                    }
                };

                var flow = new GoogleAuthorizationCodeFlow(initializer);
                var response = await flow.RefreshTokenAsync(userId, refreshToken, CancellationToken.None);

                return (WebErrorCode.None, response);
            }
            catch /*(Exception ex)*/
            {
                return (WebErrorCode.AuthTokenFailSetNx, null);
            }
        }
    }
}
