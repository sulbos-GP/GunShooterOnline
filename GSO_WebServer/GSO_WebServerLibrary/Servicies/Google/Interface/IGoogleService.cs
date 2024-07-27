using Google.Apis.Auth.OAuth2.Responses;

namespace GSO_WebServerLibrary.Servicies.Google.Interface
{
    public interface IGoogleService : IDisposable
    {
        /// GoogleService OAuth관련

        /// <summary>
        /// 서버코드를 이용하여 엑세스 토큰과 정보를 얻는다
        /// </summary>
        public Task<(WebErrorCode, TokenResponse?)> ExchangeToken(string userId, string serverCode);

        /// <summary>
        /// 엑세스 토큰을 삭제한다
        /// </summary>
        public Task<WebErrorCode> RevokeToken(string userId, string accessToken);

        /// <summary>
        /// 서버코드를 이용하여 엑세스 토큰과 정보를 얻는다
        /// </summary>
        public Task<(WebErrorCode, TokenResponse?)> RefreshToken(string userId, string refreshToken);

        /// <summary>
        /// 서버코드를 이용하여 엑세스 토큰과 정보를 얻는다
        /// </summary>
        //public Task<bool> IsValidAccessToken(String accessToken);



        /// GoogleService Game관련

        /// <summary>
        /// 엑세스 토큰을 이용하여 플레이어의 프로필 정보를 얻는다
        /// </summary>
        public Task<(WebErrorCode, Player?)> GetMyPlayer(string userId, string accessToken);
    }
}
