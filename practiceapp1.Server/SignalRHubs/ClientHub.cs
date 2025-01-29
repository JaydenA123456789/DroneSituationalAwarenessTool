using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DroneSituationalAwarenessTool.Server.SignalRHubs
{
    public class ClientHub : Hub
    {
        public async Task UpdateAddToCesium(string user, string message)
        {
            //Console.WriteLine($"1Received message from {user}: {message}");
            // Broadcast to all connected clients
            await Clients.All.SendAsync("UpdateAddToCesium", user, message);
        }
    }
}
