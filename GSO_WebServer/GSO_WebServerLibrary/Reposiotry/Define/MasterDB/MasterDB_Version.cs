using SqlKata.Execution;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using WebCommonLibrary.DTO.Middleware;

namespace GSO_WebServerLibrary.Reposiotry.Define.MasterDB
{
    public partial class MasterDB : IMasterDB
    {
        public async Task<VersionInfo> LoadLatestAppVersion()
        {
            return await mQueryFactory.Query("app_version").
                FirstOrDefaultAsync<VersionInfo>();
        }

        public async Task<VersionInfo> LoadLatestDataVersion()
        {
            return await mQueryFactory.Query("data_version").
                FirstOrDefaultAsync<VersionInfo>();
        }
    }
}
