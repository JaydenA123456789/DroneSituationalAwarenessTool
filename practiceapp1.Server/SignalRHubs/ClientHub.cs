using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DroneSituationalAwarenessTool.Server.SignalRHubs
{
    public class ClientHub : Hub
    {
        public async Task UpdateAddToCesium(string user, string message)
        {
            await Clients.All.SendAsync("UpdateAddToCesium", user, message); //Could target a single instance or allow for multiple clients to see the same data
        }
    }
}
