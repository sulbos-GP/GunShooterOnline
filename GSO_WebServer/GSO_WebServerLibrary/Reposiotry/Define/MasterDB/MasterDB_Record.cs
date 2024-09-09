using SqlKata.Execution;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using WebCommonLibrary.DTO.Middleware;
using WebCommonLibrary.Models.MasterDB;
using WebCommonLibrary.Error;

namespace GSO_WebServerLibrary.Reposiotry.Define.MasterDB
{
    public partial class MasterDB : IMasterDB
    {
        public async Task<WebErrorCode> LoadUserRating()
        {
            await Task.Delay(1000);
            return WebErrorCode.None;
        }

    }
}
