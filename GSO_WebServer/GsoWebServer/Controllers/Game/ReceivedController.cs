using GSO_WebServerLibrary.Utils;
using GsoWebServer.DTO;
using GsoWebServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.DTO.Authentication;
using WebCommonLibrary.DTO.Game;
using WebCommonLibrary.Error;

namespace GsoWebServer.Controllers.Game
{
    [Route("api/Game/[controller]")]
    [ApiController]
    public class ReceivedController : ControllerBase
    {

        private readonly IAuthenticationService mAuthenticationService;
        private readonly IGameService mGameService;

        public ReceivedController(IAuthenticationService auth, IGameService game)
        {
            mAuthenticationService = auth;
            mGameService = game;
        }

        /// <summary>
        /// 닉네임 변경
        /// </summary>
        [HttpPost]
        [Route("LevelReward")]
        public async Task<ReceivedLevelRewardRes> LevelReward([FromHeader] HeaderDTO header, [FromBody] ReceivedLevelRewardReq request)
        {

            Console.WriteLine($"[LevelReward] uid:{header.uid} level:{request.level}");

            var response = new ReceivedLevelRewardRes();

            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "";
                return response;
            }

            var errorCode = await mGameService.UpdateLevelReward(header.uid, request.level);
            if (errorCode != WebErrorCode.None)
            {
                response.error_code = errorCode;
                return response;
            }

            (errorCode, var levelReward) = await mGameService.GetUserLevelReward(header.uid, null, null);
            if (errorCode != WebErrorCode.None)
            {
                response.error_code = errorCode;
                return response;
            }

            response.error_code = WebErrorCode.None;
            response.LevelReward = levelReward;
            return response;
        }
    }
}
