using GsoWebServer.Servicies.Interfaces;
using GsoWebServer.DTO;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.DTO.Performance;
using GSO_WebServerLibrary.Utils;
using WebCommonLibrary.DTO.Game;
using WebCommonLibrary.Error;

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
            Console.WriteLine($"[PlayerStats] uid:{request.uid} room:{request.room_token}");

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

            response.error_code = WebErrorCode.None;
            return response;
        }
    }
}
