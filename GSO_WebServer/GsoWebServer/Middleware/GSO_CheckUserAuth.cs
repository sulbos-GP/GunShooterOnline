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
    public class GSO_CheckUserAuth : CheckUserAuth
    {
        public GSO_CheckUserAuth(RequestDelegate next, IMemoryDB memoryDb, IGameDB gameDB, IGoogleService googleService) : base(next, memoryDb, gameDB, googleService)
        {
            mIgnoreEndPoints = new List<string>
            {
                "/Authentication",
                "/SignIn",
            };
        }
    }
}
