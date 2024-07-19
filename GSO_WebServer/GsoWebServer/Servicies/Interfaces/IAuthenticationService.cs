using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Games.v1.Data;
using GSO_WebServerLibrary;
using static Google.Apis.Requests.RequestError;

namespace GsoWebServer.Servicies.Interfaces
{
    public interface IAuthenticationService
    {

        public Task<(WebErrorCode, TokenResponse?)> ExchangeToken(String userId, String serverCode);
        public Task<(WebErrorCode, TokenResponse?)> RefreshToken(String userId, String serverCode);
        public Task<WebErrorCode> RevokeToken(String userId, String accessToken);
        public Task<(WebErrorCode, Player?)> GetMyPlayer(String userId, String accessToken);

        /// <summary>
        /// 유저가 존재하는지 확인
        /// </summary>
        public Task<(WebErrorCode, int)> VerifyUser(String playerId, String service);

        /// <summary>
        /// 유저의 닉네임이 존재하는지 확인
        /// </summary>
        public Task<WebErrorCode> VerifyNickname(Int32 uid, String nickname);

        /// <summary>
        /// 토큰 공유 레디스에 저장
        /// </summary>
        public Task<WebErrorCode> RegisterToken(int uid, long expires, String accessToken, String refreshToken);

        /// <summary>
        /// 최근 로그인한 시간 기록
        /// </summary>
        public Task<WebErrorCode> UpdateLastSignInTime(int uid);
    }
}
