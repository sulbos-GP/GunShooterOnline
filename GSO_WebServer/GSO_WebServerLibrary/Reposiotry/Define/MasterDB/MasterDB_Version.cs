using SqlKata.Execution;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using WebCommonLibrary.DTO.Middleware;
using WebCommonLibrary.Models.MasterDB;

namespace GSO_WebServerLibrary.Reposiotry.Define.MasterDB
{
    public partial class MasterDB : IMasterDB
    {
        public async Task<DB_Version> LoadLatestAppVersion()
        {
            return await mQueryFactory.Query("app_version").
                FirstOrDefaultAsync<DB_Version>();
        }

        public async Task<DB_Version> LoadLatestDataVersion()
        {
            return await mQueryFactory.Query("data_version").
                FirstOrDefaultAsync<DB_Version>();
        }
    }
}
