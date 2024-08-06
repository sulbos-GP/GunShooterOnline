using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json.Linq;
using GSO_WebServerLibrary.Error;
using GSO_WebServerLibrary.Servicies.Interfaces;

namespace GSO_WebServerLibrary.Servicies.Google
{
    public partial class GoogleService : IGoogleService
    {
        public async Task<WebErrorCode> ValidateToken(String userId, String accessToken)
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync($"https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={accessToken}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var tokenInfo = JObject.Parse(content);

                    long expiresIn = tokenInfo.Value<long>("expires_in");

                    return (WebErrorCode.None);
                }
                else
                {
                    var errorInfo = JObject.Parse(content);

                    string? error               = errorInfo.Value<string>("error");
                    string? errorDescription    = errorInfo.Value<string>("error_description");

                    return (WebErrorCode.TEMP_ERROR);
                }
            }
            catch /*(Exception ex)*/
            {
                return (WebErrorCode.IsNotValidateServerCode);
            }
        }

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
