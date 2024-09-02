using GameServerManager.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.DTO.Matchmaker;
using WebCommonLibrary.Error;

namespace GameServerManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService mSessionService;

        public SessionController(ISessionService sessionService)
        {
            mSessionService = sessionService;
        }

        /// <summary>
        /// 가능한 매칭이 있는지 확인
        /// </summary>
        [HttpPost]
        [Route("FetchMatch")]
        public async Task<FetchMatchRes> FetchMatch([FromBody] FetchMatchReq request)
        {

            Console.WriteLine($"[FetchMatch] Requset");

            var response = new FetchMatchRes();
            try
            {

                var (error, matchProfile) = await mSessionService.FetchMatch();
                if (error != WebErrorCode.None || matchProfile == null)
                {
                    response.error_code = error;
                    response.error_description = "매치가 존재하지 않거나 오류가 발생하였습니다.";
                    return response;
                }

                response.error_code = WebErrorCode.None;
                response.match_profile = matchProfile;
                return response;
            }
            catch (Exception ex)
            {
                response.error_code = WebErrorCode.TEMP_Exception;
                response.error_description = ex.Message;
                return response;
            }

        }

        [HttpPost]
        [Route("RequestReady")]
        public async Task<RequestReadyMatchRes> RequestReady([FromBody] RequestReadyMatchReq request)
        {

            Console.WriteLine($"[RequestReady] : ContainerId : {request.container_id}");

            var response = new RequestReadyMatchRes();
            try
            {

                var error = await mSessionService.RequsetReadyMatch(request.container_id);
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

        [HttpPost]
        [Route("Shutdown")]
        public async Task<ShutdownMatchRes> Shutdown([FromBody] ShutdownMatchReq request)
        {

            Console.WriteLine($"[Shutdown] : ContainerId : {request.container_id}");

            var response = new ShutdownMatchRes();
            try
            {

                var error = await mSessionService.ShutdownMatch(request.container_id);
                if (error != WebErrorCode.None)
                {
                    response.error_code = error;
                    return response;
                }

                (error, var match) = await mSessionService.CreateMatch();
                if (error != WebErrorCode.None || match == null)
                {
                    response.error_code = error;
                    return response;
                }

                Console.WriteLine($"Shutdown : {request.container_id} -> Create : {match.ID}");

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
