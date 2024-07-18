using AuthenticationServer.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.Extensions.Options;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using System.Net;
using Google.Apis.Games.v1;
using Google.Apis.Games.v1.Data;

namespace AuthenticationServer.Service
{
    public class GoogleService : IService
    {
        private readonly IOptions<GoogleConfig> mGoogleConfig;

        public GoogleService(IOptions<GoogleConfig> config)
        {
            mGoogleConfig = config;
        }

        public async Task<TokenResponse> AccessToken(String userId, String serverCode)
        {
            var response = new TokenResponse();
            try
            {
                string[] scopes = { "openid", "profile", "email"};
                var initializer = new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = mGoogleConfig.Value.ClientId,
                        ClientSecret = mGoogleConfig.Value.ClientSecret,
                    },
                    Scopes = scopes
                };

                var flow = new GoogleAuthorizationCodeFlow(initializer);
                response = await flow.ExchangeCodeForTokenAsync(userId, serverCode, mGoogleConfig.Value.RedirectUri, CancellationToken.None);

                response = await RefreshToken(userId, response.RefreshToken);

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AsscessToken] error:{ex.ToString()}");
                return response;
            }
        }

        public async Task RevokeToken(String userId, String accessToken)
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RevokeToken] error:{ex.ToString()}");
            }
        }

        public async Task<TokenResponse> RefreshToken(String userId, String refreshToken)
        {
            var response = new TokenResponse();
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
                response = await flow.RefreshTokenAsync(userId, refreshToken, CancellationToken.None);

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RefreshToken] error:{ex.ToString()}");
                return response;
            }
        }

        public async Task<Player> GetPlayer(String accessToken)
        {
            var response = new Player();
            try
            {
                var credential = GoogleCredential.FromAccessToken(accessToken);

                var service = new GamesService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = mGoogleConfig.Value.ApplicationName
                });

                PlayersResource.GetRequest request = service.Players.Get("me");
                response = await request.ExecuteAsync();

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetPlayer] error:{ex.ToString()}");
                return response;
            }
        }

        public async Task<bool> IsValidAccessToken(String accessToken)
        {
            try
            {

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://oauth2.googleapis.com/tokeninfo?access_token={accessToken}");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();

                        if(mGoogleConfig.Value.ClientId == null)
                        {
                            return false;
                        }

                        if (responseContent.Contains(mGoogleConfig.Value.ClientId))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Console.WriteLine("Access token expired. Refresh token and retry.");
                        return false;
                    }
                    else
                    {
                        Console.WriteLine($"Token info request failed with status code {response.StatusCode}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IsValidAccessToken] error:{ex.ToString()}");
                return false;
            }
        }

        public void Dispose()
        {
            
        }
    }
}
