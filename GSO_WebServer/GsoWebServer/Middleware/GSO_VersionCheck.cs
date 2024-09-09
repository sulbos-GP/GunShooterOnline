using GSO_WebServerLibrary.Reposiotry.Interfaces;
using GsoWebServer.Servicies.Interfaces;
using System.Text.Json;
using WebCommonLibrary.Error;
using WebCommonLibrary.DTO.Middleware;
using WebCommonLibrary.Models.MemoryDB;
using GSO_WebServerLibrary.Middleware;
using GSO_WebServerLibrary.Reposiotry.Define.GameDB;
using GSO_WebServerLibrary.Reposiotry.Define.MemoryDB;
using GSO_WebServerLibrary.Servicies.Interfaces;

namespace GsoWebServer.Middleware
{
    public class GSO_VersionCheck : VersionCheck
    {
        public GSO_VersionCheck(RequestDelegate next, IMasterDB masterDb) : base(next, masterDb)
        {
            mUseEndPoints = new List<string>
            {
                "/SignIn"
            };
        }
    }
}
