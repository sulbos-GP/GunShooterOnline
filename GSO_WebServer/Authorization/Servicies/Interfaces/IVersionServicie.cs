using Google.Apis.Auth.OAuth2.Responses;
using GSO_WebServerLibrary.Error;

namespace Authorization.Servicies.Interfaces
{
    public interface IVersionServicie : IDisposable
    {
        /// <summary>
        /// 가장 최근의 앱 버전을 불러온다
        /// </summary>
        public Task<Version?> GetLatestAppVersion();

        /// <summary>
        /// 가장 최근의 데이터 버전을 불러온다
        /// </summary>
        public Task<Version?> GetLatestDataVersion();

    }
}
