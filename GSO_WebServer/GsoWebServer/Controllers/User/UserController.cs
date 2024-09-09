using GSO_WebServerLibrary.Utils;
using GsoWebServer.DTO;
using GsoWebServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.DTO.User;
using WebCommonLibrary.Error;

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
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "요청이 올바르지 않습니다.";
                return response;
            }

            //이전 닉네임과 일치하는지 DB에서 체크
            var error = await mAuthenticationService.VerifyNickname(header.uid, request.new_nickname);
            if (error != WebErrorCode.None)
            {
                response.error_code = error;
                response.error_description = "이전 닉네임과 일치 합니다.";
                return response;
            }

            //DB에 닉네임 업데이트
            (error, var nickname) = await mGameService.UpdateNickname(header.uid, request.new_nickname);
            if (error != WebErrorCode.None)
            {
                response.error_code = error;
                response.error_description = "이미 사용중인 닉네임 입니다.";
                return response;
            }

            response.error_code = WebErrorCode.None;
            response.nickname = nickname;
            return response;
        }


    }
}
