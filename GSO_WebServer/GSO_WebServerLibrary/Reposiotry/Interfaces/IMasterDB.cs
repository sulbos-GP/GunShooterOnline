using WebCommonLibrary.DTO.Middleware;

namespace GSO_WebServerLibrary.Reposiotry.Interfaces
{
    public interface IMasterDB : IDisposable
    {
        /// <summary>
        /// 가장 최근의 앱 버전을 불러온다
        /// </summary>
        /// 
        public Task<VersionInfo> LoadLatestAppVersion();

        /// <summary>
        /// 가장 최근의 데이터 버전을 불러온다
        /// </summary>
        public Task<VersionInfo> LoadLatestDataVersion();
    }
}
