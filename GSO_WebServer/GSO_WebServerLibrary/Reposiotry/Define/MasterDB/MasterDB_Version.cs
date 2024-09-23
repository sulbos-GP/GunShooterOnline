using SqlKata.Execution;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using WebCommonLibrary.DTO.Middleware;
using WebCommonLibrary.Models.MasterDatabase;
namespace GSO_WebServerLibrary.Reposiotry.Define.MasterDB
{
    public partial class MasterDB : IMasterDB
    {
        public async Task<FMasterVersionApp> LoadLatestAppVersion()
        {
            return await mQueryFactory.Query("master_version_app").
                FirstOrDefaultAsync<FMasterVersionApp>();
        }

        public async Task<FMasterVersionData> LoadLatestDataVersion()
        {
            return await mQueryFactory.Query("master_version_data").
                FirstOrDefaultAsync<FMasterVersionData>();
        }
    }
}
