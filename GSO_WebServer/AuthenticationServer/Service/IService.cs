using AuthenticationServer.Models;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Games.v1.Data;
using Google.Apis.Oauth2.v2.Data;

namespace AuthenticationServer.Service
{
    public interface IService : IDisposable
    {
        public Task<TokenResponse> AccessToken(String userId, String serverCode);
        public Task RevokeToken(String userId, String accessToken);
        public Task<TokenResponse> RefreshToken(String userId, String refreshToken);
        public Task<Player> GetPlayer(String accessToken);

        public Task<bool> IsValidAccessToken(String accessToken);
    }
}
