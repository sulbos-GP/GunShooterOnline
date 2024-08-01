using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Matchmaker.Models;
using Matchmaker.Service.Interfaces;
using Matchmaker.DTO;
using Matchmaker.DTO.Matchmaker;
using GSO_WebServerLibrary.Error;
using GSO_WebServerLibrary.Utils;
using GSO_WebServerLibrary.DTO;

namespace Matchmaker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchmakerController : ControllerBase
    {
        private readonly IMatchmakerService mMatchmakerService;

        public MatchmakerController(IMatchmakerService matchmakerService)
        {
            mMatchmakerService = matchmakerService;
        }

        /// <summary>
        /// 매칭 시작
        /// </summary>
        [HttpPost]
        [Route("Join")]
        public async Task<JoinMatchRes> Join([FromHeader] HeaderDTO header, [FromBody] JoinMatchReq request)
        {

            var response = new JoinMatchRes();

            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                return response;
            }

            var error = await mMatchmakerService.PushMatchQueue(header.uid, request.world, request.region);
            if(error != WebErrorCode.None)
            {
                response.error_code = error;
                return response;
            }

            response.error_code = WebErrorCode.None;
            return response;
        }

        /// <summary>
        /// 매칭 취소
        /// </summary>
        [HttpPost]
        [Route("Cancle")]
        public async Task<CancleMatchRes> Cancle([FromHeader] HeaderDTO header, [FromBody] CancleMatchReq request)
        {

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
