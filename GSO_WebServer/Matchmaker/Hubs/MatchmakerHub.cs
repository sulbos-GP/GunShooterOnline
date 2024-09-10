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
            Console.WriteLine($"Client connected: {connectionId}");

            await base.OnConnectedAsync();

        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {

            string connectionId = Context.ConnectionId;
            Console.WriteLine($"Client disconnected: {connectionId}");

            var error = await mMatchmakerService.RemoveMatchTicket(connectionId);

            await base.OnDisconnectedAsync(exception);
        }
        
        //임시
        public async Task C2S_VerfiyUser(int uid, string accessToken)
        {
            string connectionId = Context.ConnectionId;

            var error = await mMatchmakerService.AddMatchTicket(uid, connectionId);
            await Clients.Caller.SendAsync("S2C_VerfiyUser", error);
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
