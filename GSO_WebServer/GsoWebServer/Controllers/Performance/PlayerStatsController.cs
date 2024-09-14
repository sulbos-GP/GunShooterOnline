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
        private readonly IPlayerPerformanceService mRatingSystem;

        public PlayerStatsController(IPlayerPerformanceService ratingSystem)
        {
            mRatingSystem = ratingSystem;
        }

        [HttpPost]
        public async Task<PlayerStatsRes> PlayerStats([FromHeader] HeaderDTO header, [FromBody] PlayerStatsReq request)
        {
            Console.WriteLine($"[PlayerStats] uid:{header.uid} room:{request.room_token}");

            var response = new PlayerStatsRes();

            if (!WebUtils.IsValidModelState(request) || request.outcome == null)
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "";
                return response;
            }

            var errorCode = await mRatingSystem.UpdatePlayerStats(header.uid, request.outcome);
            if (errorCode != WebErrorCode.None)
            {
                response.error_code = errorCode;
                return response;
            }

            response.error_code = WebErrorCode.None;
            return response;
        }
    }
}
