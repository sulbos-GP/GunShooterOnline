using GSO_WebServerLibrary.Utils;
using GsoWebServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.DTO.User;
using WebCommonLibrary.Error;
using GsoWebServer.DTO;
using static Google.Apis.Requests.RequestError;
using GsoWebServer.Servicies.DataLoad;

namespace GsoWebServer.Controllers.User
{
    [Route("api/User/[controller]")]
    [ApiController]
    public class DailyTaskController : ControllerBase
    {
        private readonly IAuthenticationService mAuthenticationService;
        private readonly IGameService mGameService;
        private readonly IDataLoadService mDataLoadService;

        public DailyTaskController(IAuthenticationService auth, IGameService game, IDataLoadService dataload)
        {
            mAuthenticationService = auth;
            mGameService = game;
            mDataLoadService = dataload;
        }

        [HttpPost]
        [Route("Update")]
        public async Task<DailyTaskRes> Update([FromHeader] HeaderDTO header, [FromBody] DailyTaskReq request)
        {
            Console.WriteLine($"[User.DailyTask.Update] uid:{header.uid}");

            var response = new DailyTaskRes();

            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "";
                return response;
            }
            int uid = header.uid;

            var errorCode = await mGameService.UpdateDailyTask(uid);
            if (errorCode != WebErrorCode.None)
            {
                response.error_code = errorCode;
                response.error_description = "";
                return response;
            }

            //최근 로그인 시간 변경
            errorCode = await mAuthenticationService.UpdateLastSignInTime(uid);
            if (errorCode != WebErrorCode.None)
            {
                response.error_code = errorCode;
                response.error_description = "";
                return response;
            }

            (errorCode, response.DailyLoads) = await mDataLoadService.DailyLoadData(uid);
            if (errorCode != WebErrorCode.None)
            {
                response.error_code = errorCode;
                response.error_description = "";
                return response;
            }

            response.error_code = WebErrorCode.None;
            return response;
        }
    }
}
