using GSO_WebServerLibrary.Utils;
using GsoWebServer.Servicies.Interfaces;
using GsoWebServer.DTO;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.DTO.User;
using WebCommonLibrary.Error;

namespace GsoWebServer.Controllers.User
{
    [Route("api/User/[controller]")]
    [ApiController]
    public class SetNicknameController : ControllerBase
    {
        private readonly IAuthenticationService mAuthenticationService;
        private readonly IGameService mGameService;

        public SetNicknameController(IAuthenticationService auth, IGameService game)
        {
            mAuthenticationService = auth;
            mGameService = game;
        }

        /// <summary>
        /// 닉네임 변경
        /// </summary>
        [HttpPost]
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
            var (error, user) = await mGameService.GetUserInfo(header.uid);
            if (error != WebErrorCode.None || user == null)
            {
                response.error_code = error;
                response.error_description = "유저에 대한 정보가 없습니다.";
                return response;
            }

            if(user.nickname == request.new_nickname)
            {
                response.error_code = WebErrorCode.SetNicknameFailSameNickname;
                response.error_description = "현재 닉네임과 이전 닉네임이 같습니다.";
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
            user.nickname = nickname;

            response.error_code = WebErrorCode.None;
            response.error_description = "성공적으로 닉네임을 변경하였습니다.";
            response.user = user;
            return response;
        }
    }
}
