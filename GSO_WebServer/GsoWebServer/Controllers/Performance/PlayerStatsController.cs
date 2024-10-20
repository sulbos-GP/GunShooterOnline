using GsoWebServer.Servicies.Interfaces;
using GsoWebServer.DTO;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.DTO.Performance;
using GSO_WebServerLibrary.Utils;
using WebCommonLibrary.DTO.Game;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;

namespace GsoWebServer.Controllers.Performance
{
    [Route("api/Performance/[controller]")]
    [ApiController]
    public class PlayerStatsController : ControllerBase
    {
        private readonly IGameService mGameService;
        private readonly IPlayerPerformanceService mPerformanceService;

        public PlayerStatsController(IGameService gameService, IPlayerPerformanceService performanceService)
        {
            mGameService = gameService;
            mPerformanceService = performanceService;
        }

        [HttpPost]
        public async Task<PlayerStatsRes> PlayerStats([FromBody] PlayerStatsReq request)
        {

            if(request.outcome == null)
            {
                request.outcome = new MatchOutcome();
            }

            Console.WriteLine($"[PlayerStats] uid:{request.uid} room:{request.room_token} : " +
                $"Outcome" +
                $"(" +
                $"kills:{request.outcome.kills}" +
                $"death:{request.outcome.death}" +
                $"damage:{request.outcome.damage}" +
                $"farming:{request.outcome.farming}" +
                $"escape:{request.outcome.escape}" +
                $"survival_time:{request.outcome.survival_time}" +
                $")");

            var response = new PlayerStatsRes();

            if (!WebUtils.IsValidModelState(request) || request.outcome == null)
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "";
                return response;
            }

            var error = await mPerformanceService.UpdatePlayerStats(request.uid, request.outcome);
            if (error != WebErrorCode.None)
            {
                response.error_code = error;
                return response;
            }

            var experience = mPerformanceService.CalculateExperience(request.outcome);

            error = await mGameService.UpdateLevel(request.uid, experience);
            if (error != WebErrorCode.None)
            {
                response.error_code = error;
                return response;
            }

            (error, response.User) = await mGameService.GetUserInfo(request.uid);
            if (error != WebErrorCode.None || response.User == null)
            {
                response.error_code = error;
                return response;
            }

            response.error_code = WebErrorCode.None;
            return response;
        }
    }
}
