using GSO_WebServerLibrary.Utils;
using Matchmaker.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.DTO.Matchmaker;
using WebCommonLibrary.Error;
using Matchmaker.DTO;

namespace Matchmaker.Controllers
{
    [Route("api/Matchmaker/[controller]")]
    [ApiController]
    public class JoinController : ControllerBase
    {
        private readonly IMatchmakerService mMatchmakerService;

        public JoinController(IMatchmakerService matchmakerService)
        {
            mMatchmakerService = matchmakerService;
        }

        /// <summary>
        /// 매칭 참여 요청
        /// </summary>
        [HttpPost]
        public async Task<JoinMatchRes> Join([FromHeader] HeaderDTO header, [FromBody] JoinMatchReq request)
        {

            Console.WriteLine($"[Join] uid:{header.uid} world:{request.world} region:{request.region}");

            var response = new JoinMatchRes();
            try
            {
                if (!WebUtils.IsValidModelState(request))
                {
                    response.error_code = WebErrorCode.IsNotValidModelState;
                    return response;
                }

                var error = await mMatchmakerService.PushMatchQueue(header.uid, request.world, request.region);
                if (error != WebErrorCode.None)
                {
                    response.error_code = error;
                    return response;
                }

                response.error_code = WebErrorCode.None;
                return response;
            }
            catch (Exception ex)
            {
                response.error_code = WebErrorCode.TEMP_Exception;
                response.error_description = ex.Message;
                return response;
            }
        }
    }
}
