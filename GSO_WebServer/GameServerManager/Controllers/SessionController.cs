using GameServerManager.Servicies;
using GameServerManager.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using WebCommonLibrary.DTO.GameServer;
using WebCommonLibrary.DTO.Matchmaker;
using WebCommonLibrary.Error;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var response = new FetchMatchRes();

            Console.WriteLine($"[FetchMatch] {request.playerCount}");
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
        [Route("DispatchMatchPlayers")]
        public async Task<DispatchMatchPlayerRes> DispatchMatchPlayers([FromBody] DispatchMatchPlayerReq request)
        {

            var response = new DispatchMatchPlayerRes();
            if (request.match_profile == null || request.players == null)
            {
                response.error_code = WebErrorCode.TEMP_ERROR;
                return response;
            }

            Console.WriteLine($"[DispatchMatchPlayers] {request.match_profile.container_id}");
            try
            {

                var (error, matchProfile) = await mSessionService.GetMatchStatus(request.match_profile.container_id);
                if (error != WebErrorCode.None || matchProfile == null || matchProfile.players != null)
                {
                    response.error_code = error;
                    response.error_description = "매치가 존재하지 않거나 오류가 발생하였습니다.";
                    return response;
                }

                error = await mSessionService.DispatchMatchPlayers(request.match_profile.container_id, request.players);
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

        //[HttpPost]
        //[Route("DispatchMatchPlayer")]
        //public async Task<DispatchMatchPlayerRes> DispatchMatchPlayer([FromBody] DispatchMatchPlayerReq request)
        //{
        //    var response = new DispatchMatchPlayerRes();

        //    if(request.players == null)
        //    {
        //        response.error_code = WebErrorCode.IsNotValidModelState;
        //        return response;
        //    }

        //    var error = await mSessionService.DispatchMatchPlayers(request.container_id, request.players);
        //    if (error != WebErrorCode.None)
        //    {
        //        response.error_code = error;
        //        return response;
        //    }

        //    response.error_code = WebErrorCode.None;
        //    return response;
        //}


        [HttpPost]
        [Route("AllocateMatch")]
        public async Task<AllocateMatchRes> AllocateMatch([FromBody] AllocateMatchReq request)
        {
            Console.WriteLine($"[AllocateMatch] : ContainerId : {request.container_id}");

            var response = new AllocateMatchRes();

            try
            {
                var (error, status) = await mSessionService.GetMatchStatus(request.container_id);
                if (error != WebErrorCode.None || status == null)
                {
                    response.error_code = error;
                    return response;
                }


                if (status.players == null)
                {
                    response.error_code = WebErrorCode.TEMP_ERROR;
                    return response;
                }

                response.players = status.players;
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
        [Route("StartMatch")]
        public async Task<StartMatchRes> StartMatch([FromBody] StartMatchReq request)
        {
            Console.WriteLine($"[StartMatch] : ContainerId : {request.container_id}");

            var response = new StartMatchRes();

            try
            {
                var (error, status) = await mSessionService.GetMatchStatus(request.container_id);
                if (error != WebErrorCode.None || status == null)
                {
                    response.error_code = error;
                    return response;
                }

                error = await mSessionService.StartMatch(request.container_id, status);
                if (error != WebErrorCode.None)
                {
                    response.error_code = error;
                    return response;
                }

                return response;
            }
            catch (Exception ex)
            {
                response.error_code = WebErrorCode.TEMP_Exception;
                response.error_description = ex.Message;
                return response;
            }
        }

        /// <summary>
        /// 컨테이너 소켓 서버에서 플레이어 올때까지 대기
        /// </summary>
        //[HttpPost]
        //[Route("MatchPlayers")]
        //public async Task<MatchPlayersRes> MatchPlayers([FromBody] MatchPlayersReq request, CancellationToken cancellationToken)
        //{

        //    Console.WriteLine($"[MatchPlayers] : ContainerId : {request.container_id}");

        //    var taskCompletionSource = new TaskCompletionSource<IActionResult>();
        //    var response = new MatchPlayersRes();

        //    var pollingTask = Task.Run(async () =>
        //    {
        //        while (!cancellationToken.IsCancellationRequested)
        //        {
        //            var (error, status) = await mSessionService.GetMatchStatus(request.container_id);
        //            if (error != WebErrorCode.None || status == null)
        //            {
        //                taskCompletionSource.SetResult(NoContent());
        //                return;
        //            }

        //            if (status.players != null)
        //            {
        //                response.players = status.players;
        //                taskCompletionSource.SetResult(Ok());
        //                break;
        //            }


        //            await Task.Delay(500, cancellationToken);
        //        }
        //    }, cancellationToken);

        //    cancellationToken.Register(() => taskCompletionSource.SetResult(NoContent()));

        //    await taskCompletionSource.Task;

        //    return response;
        //}

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
