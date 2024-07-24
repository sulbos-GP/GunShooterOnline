using Microsoft.AspNetCore.SignalR;

namespace Matchmaker.SignalRHub
{
    public class ClientHub : Hub
    {
        public async Task SendMessageToSpecificUser<TMessage>(string method, string user, TMessage message)
        {
            await Clients.User(user).SendAsync(method, user, message);
        }

        public async Task SendMessageToGroup<TMessage>(string group, string method, string[] users, TMessage message)
        {
            foreach (var user in users) 
            {
                await Groups.AddToGroupAsync(user, group);
            }

            await Clients.Group(group).SendAsync(method, message);

            foreach (var user in users)
            {
                await Groups.RemoveFromGroupAsync(user, group);
            }
        }
    }
}
