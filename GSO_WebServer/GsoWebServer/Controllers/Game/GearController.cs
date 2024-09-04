using GSO_WebServerLibrary.Utils;
using GsoWebServer.DTO;
using GsoWebServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.DTO.Game;
using WebCommonLibrary.Error;

namespace GsoWebServer.Controllers.Game
{
    [Route("api/Game/[controller]")]
    [ApiController]
    public class GearController : ControllerBase
    {
        private readonly IAuthenticationService mAuthenticationService;
        private readonly IGameService mGameService;

        public GearController(IAuthenticationService auth, IGameService game)
        {
            mAuthenticationService = auth;
            mGameService = game;
        }

        [HttpPost]
        [Route("Load")]
        public async Task<LoadGearRes> Load([FromHeader] HeaderDTO header, [FromBody] LoadGearReq request)
        {
            Console.WriteLine($"[Game.Gear.Load] uid:{header.uid}");

            var response = new LoadGearRes();

            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "요청이 올바르지 않습니다.";
                return response;
            }

            var (error, gears) = await mGameService.LoadGear(header.uid);
            if (error != WebErrorCode.None || gears == null)
            {
                response.error_code = error;
                response.error_description = "저장소 아이디가 존재하지 않습니다.";
                return response;
            }

            response.error_code = WebErrorCode.None;
            response.error_description = "";
            response.gears = gears.ToList();
            return response;


        }
    }
}
