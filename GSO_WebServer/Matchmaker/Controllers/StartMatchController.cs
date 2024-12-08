using GSO_WebServerLibrary.Reposiotry.Interfaces;
using GSO_WebServerLibrary.Utils;
using Matchmaker.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.DTO.GameServer;
using WebCommonLibrary.DTO.Matchmaker;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.Match;

namespace Matchmaker.Controllers
{
    [Route("api/Session/[controller]")]
    [ApiController]
    public class StartMatchController : ControllerBase
    {
        private readonly IMatchmakerService mMatchmakerService;

        public StartMatchController(IMatchmakerService matchmakerService)
        {
            mMatchmakerService = matchmakerService;
        }

        /// <summary>
        /// 매칭 참여 요청
        /// </summary>
        [HttpPost]
        public async Task<NotifyStartMatchRes> StartMatch([FromBody] NotifyStartMatchReq request)
        {

            Console.WriteLine($"[StartMatch]");

            var response = new NotifyStartMatchRes();
            try
            {
                if (!WebUtils.IsValidModelState(request))
                {
                    response.error_code = WebErrorCode.IsNotValidModelState;
                    return response;
                }

                if(request.players == null || request.match_profile == null)
                {
                    response.error_code = WebErrorCode.IsNotValidModelState;
                    return response;
                }

                Console.WriteLine("MatchInfo");
                Console.WriteLine("{");
                Console.WriteLine($"\tID      : {request.match_profile.container_id}");
                Console.WriteLine($"\tWORLD   : {request.match_profile.world}");
                Console.WriteLine($"\tH_IP    : {request.match_profile.host_ip}");
                Console.WriteLine($"\tH_PORT  : {request.match_profile.host_port}");
                Console.WriteLine($"\tC_PORT  : {request.match_profile.container_port}");


                //해당 클라이언트에게 방 정보 전송
                Console.WriteLine("\tMatchPlayers");
                Console.WriteLine("\t{");
                foreach (var uid in request.players)
                {
                    string key = KeyUtils.MakeKey(KeyUtils.EKey.MATCH, uid);
                    Ticket? ticket = await mMatchmakerService.GetMatchTicket(uid);
                    if(ticket == null)
                    {
                        continue;
                    }

                    //이미 매치가 잡혔기 때문에 닷지 실패
                    if (ticket.isExit == true)
                    {
                        await mMatchmakerService.NotifyMatchFailed(ticket, WebErrorCode.PopPlayersJoinForced);
                    }
                    else
                    {
                        await mMatchmakerService.NotifyMatchSuccess(uid, ticket, request.match_profile);
                    }

                    var error = await mMatchmakerService.RemoveMatchQueue(key);
                    if (error != WebErrorCode.None)
                    {
                        response.error_code = error;
                        return response;
                    }

                    Console.WriteLine("\t}");
                    Console.WriteLine("}");
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
