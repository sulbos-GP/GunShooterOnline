using GSO_WebServerLibrary.Utils;
using Matchmaker.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.DTO.Matchmaker;
using WebCommonLibrary.Error;
using Matchmaker.DTO;

namespace Matchmaker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancleController : ControllerBase
    {
        private readonly IMatchmakerService mMatchmakerService;

        public CancleController(IMatchmakerService matchmakerService)
        {
            mMatchmakerService = matchmakerService;
        }

        /// <summary>
        /// 매칭 취소 요청
        /// </summary>
        [HttpPost]
        [Route("Cancle")]
        public async Task<CancleMatchRes> Cancle([FromHeader] HeaderDTO header, [FromBody] CancleMatchReq request)
        {

            Console.WriteLine($"[Cancle] uid:{header.uid} world:{request.world} region:{request.region}");
            var response = new CancleMatchRes();

            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                return response;
            }

            var error = await mMatchmakerService.PopMatchQueue(header.uid);
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
