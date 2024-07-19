using GSO_WebServerLibrary;
using GsoWebServer.DTO;
using GsoWebServer.DTO.Authentication;
using GsoWebServer.DTO.User;
using GsoWebServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GsoWebServer.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthenticationService mAuthenticationService;
        private readonly IGameService mGameService;

        public UserController(IAuthenticationService auth, IGameService game)
        {
            mAuthenticationService = auth;
            mGameService = game;
        }

        /// <summary>
        /// 닉네임 변경
        /// </summary>
        [HttpPost]
        [Route("SetNickname")]
        public async Task<SetNicknameRes> SetNickname([FromHeader] HeaderDTO header, [FromBody] SetNicknameReq request)
        {
            Console.WriteLine($"[SetNickname] uid:{header.uid} new:{request.new_nickname}");

            var response = new SetNicknameRes();

            if (!WebUtils.IsValidModelState(request))
            {
                response.error = WebErrorCode.IsNotValidModelState;
                return response;
            }

            //이전 닉네임과 일치하는지 DB에서 체크
            var error = await mAuthenticationService.VerifyNickname(header.uid, request.new_nickname);
            if(error != WebErrorCode.None)
            {
                response.error = error;
                return response;
            }

            //DB에 닉네임 업데이트
            (error, var nickname) = await mGameService.UpdateNickname(header.uid, request.new_nickname);
            if (error != WebErrorCode.None)
            {
                response.error = error;
                return response;
            }

            response.error = WebErrorCode.None;
            response.nickname = nickname;
            return response;
        }
    }
}
