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
    public class DailyQuestController : ControllerBase
    {
        private readonly IAuthenticationService mAuthenticationService;
        private readonly IGameService mGameService;
        private readonly IDataLoadService mDataLoadService;

        public DailyQuestController(IAuthenticationService auth, IGameService game, IDataLoadService dataload)
        {
            mAuthenticationService = auth;
            mGameService = game;
            mDataLoadService = dataload;
        }

        [HttpPost]
        [Route("Complete")]
        public async Task<DailyQuestRes> Complete([FromHeader] HeaderDTO header, [FromBody] DailyQuestReq request)
        {
            Console.WriteLine($"[User.DailyQuest.Complete] uid:{header.uid} quest:{request.QuestId}");

            var response = new DailyQuestRes();

            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "";
                return response;
            }
            int uid = header.uid;
            int qeust_id = request.QuestId;

            var error = await mGameService.CompleteDailyQuset(uid, qeust_id);
            if (error != WebErrorCode.None)
            {
                response.error_code = error;
                response.error_description = "";
                return response;
            }

            (error, var user) = await mGameService.GetUserInfo(uid);
            if (error != WebErrorCode.None)
            {
                response.error_code = error;
                response.error_description = "";
                return response;
            }

            (error, var dailyQuest) = await mGameService.GetDailyQuest(uid);
            if (error != WebErrorCode.None)
            {
                response.error_code = error;
                response.error_description = "";
                return response;
            }

            response.error_code = WebErrorCode.None;
            response.User = user;
            response.DailyQuset = dailyQuest;
            return response;
        }
    }
}
