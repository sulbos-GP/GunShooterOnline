using GsoWebServer.Servicies.Interfaces;
using GsoWebServer.DTO;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.DTO.Performance;
using GSO_WebServerLibrary.Utils;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;

namespace GsoWebServer.Controllers.Performance
{
    [Route("api/Performance/[controller]")]
    [ApiController]
    public class PlayerRatingController : ControllerBase
    {
        private readonly IPlayerPerformanceService mPerformanceService;

        public PlayerRatingController(IPlayerPerformanceService performanceService)
        {
            mPerformanceService = performanceService;
        }


        [HttpPost]
        public async Task<PlayerRatingRes> PlayerRating([FromHeader] HeaderDTO header, [FromBody] PlayerRatingReq request)
        {
            Console.WriteLine($"[PlayerRating] uid:{header.uid} room:{request.room_token}");

            var response = new PlayerRatingRes();

            if (!WebUtils.IsValidModelState(request) || request.outcomes == null)
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "";
                return response;
            }

            var error = await mPerformanceService.UpdatePlayerRating(request.outcomes);
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
