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
    public class StateController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IGameService gameService;

        public StateController(IAuthenticationService auth, IGameService game)
        {
            authenticationService = auth;
            gameService = game;
        }

        [HttpPost]
        [Route("Update")]
        public async Task<UpdateUserStateRes> Update([FromHeader] HeaderDTO header, [FromBody] UpdateUserStateReq request)
        {
            Console.WriteLine($"[User.State.Update] uid:{header.uid} state:{request.state.ToString()}");

            var response = new UpdateUserStateRes();

            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "";
                return response;
            }

            //

            //


            response.error_code = WebErrorCode.None;
            return response;
        }
    }
}
