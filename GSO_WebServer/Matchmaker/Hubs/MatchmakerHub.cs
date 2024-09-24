using WebCommonLibrary.DTO.Matchmaker;
using Matchmaker.Service;
using Matchmaker.Service.Interfaces;
using Microsoft.AspNetCore.SignalR;
using ZLogger;

namespace Matchmaker.Hubs
{
    public class MatchmakerHub : Hub
    {
        private readonly IMatchmakerService mMatchmakerService;

        public MatchmakerHub(IMatchmakerService matchmakerService)
        {
            mMatchmakerService = matchmakerService;
        }

        public override async Task OnConnectedAsync()
        {

            string connectionId = Context.ConnectionId;
            Console.WriteLine($"[MatchmakerHub] Client connected: {connectionId}");

            HttpContext? context = Context.GetHttpContext();
            if (context != null)
            {
                string uid = context.Request.Headers["uid"].ToString();
                string accessToken = context.Request.Headers["access_token"].ToString();

                var error = await mMatchmakerService.AddMatchTicket(Convert.ToInt32(uid), connectionId);
            }

            await base.OnConnectedAsync();

        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {

            string connectionId = Context.ConnectionId;
            Console.WriteLine($"[MatchmakerHub] Client disconnected: {connectionId}");

            var error = await mMatchmakerService.DisconnectMatch(connectionId);

            await base.OnDisconnectedAsync(exception);
        }

        //임시
        public async Task C2S_Ping(int uid, long timestamp, long avgLatency)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long latency = now - timestamp;

            string connectionId = Context.ConnectionId;

            await mMatchmakerService.UpdateLatency(uid, avgLatency);

            //await Clients.Caller.SendAsync("S2C_Pong", timestamp);
        }

    }
}
