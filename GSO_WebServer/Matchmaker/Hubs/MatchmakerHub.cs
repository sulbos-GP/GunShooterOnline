using Matchmaker.DTO.Matchmaker;
using Microsoft.AspNetCore.SignalR;

namespace Matchmaker.Hubs
{
    public class MatchmakerHub : Hub
    {

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

            await base.OnDisconnectedAsync(exception);
        }

        public async Task Ping(long timestamp)
        {
            await Clients.Caller.SendAsync("Pong", timestamp);
        }

        /// <summary>
        /// 매치가 완료되었다는 것을 알린다
        /// </summary>
        public async Task NotifyMatchComplete(string clientId, RoomInfo info)
        {
            await Clients.Client(clientId).SendAsync("ReceiveMatchComplete", info);
        }

    }
}
