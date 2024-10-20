using GSO_WebServerLibrary.Utils;
using GsoWebServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.DTO.User;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDatabase;
using GsoWebServer.DTO;
using WebCommonLibrary.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GsoWebServer.Controllers.User
{
    [Route("api/User/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly IAuthenticationService mAuthenticationService;
        private readonly IGameService mGameService;

        public TicketController(IAuthenticationService auth, IGameService game)
        {
            mAuthenticationService = auth;
            mGameService = game;
        }

        [HttpPost]
        [Route("Update")]
        public async Task<UpdateTicketRes> Update([FromHeader] HeaderDTO header, [FromBody] UpdateTicketReq request)
        {
            Console.WriteLine($"[User.Ticket.Update] uid:{header.uid}");

            var response = new UpdateTicketRes();

            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "";
                return response;
            }

            var (error, user) = await mGameService.GetUserInfo(header.uid);
            if (error != WebErrorCode.None || user == null)
            {
                response.error_code = error;
                return response;
            }

            if(user.ticket >= GameDefine.MAX_TICKET)
            {
                response.error_code = WebErrorCode.TicketAlreadyMax;
                return response;
            }

            error = await mGameService.UpdateTicketCount(header.uid);
            if (error == WebErrorCode.TicketRemainingTime)
            {
                DateTime now = user.recent_ticket_dt;
                DateTime next = now.AddMinutes(GameDefine.WAIT_TICKET_MINUTE);
                TimeSpan timeRemaining = next - now;

                response.RemainingTime = (int)timeRemaining.TotalSeconds;
                response.error_code = WebErrorCode.TicketRemainingTime;
                return response;
            }
            else if (error != WebErrorCode.None)
            {
                response.error_code = error;
                return response;
            }

            (error, user) = await mGameService.GetUserInfo(header.uid);
            if (error != WebErrorCode.None || user == null)
            {
                response.error_code = error;
                return response;
            }

            response.User = user;
            response.error_code = WebErrorCode.None;
            return response;
        }
    }

}
