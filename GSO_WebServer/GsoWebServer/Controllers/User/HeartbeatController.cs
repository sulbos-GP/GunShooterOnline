using GSO_WebServerLibrary.Utils;
using GsoWebServer.Servicies.Interfaces;
using GsoWebServer.DTO;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.Error;
using WebCommonLibrary.DTO.User;

namespace GsoWebServer.Controllers.User
{
    [Route("api/User/[controller]")]
    [ApiController]
    public class HeartbeatController : ControllerBase
    {
        private readonly IAuthenticationService mAuthenticationService;
        private readonly IGameService mGameService;

        public HeartbeatController(IAuthenticationService auth, IGameService game)
        {
            mAuthenticationService = auth;
            mGameService = game;
        }

        [HttpPost]
        public async Task<HeartbeatRes> Heartbeat([FromHeader] HeaderDTO header, [FromBody] HeartbeatReq request)
        {
            Console.WriteLine($"[user.Heartbeat] uid:{header.uid}");

            var response = new HeartbeatRes();

            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "";
                return response;
            }



            response.error_code = WebErrorCode.None;
            return response;
        }
    }
}
