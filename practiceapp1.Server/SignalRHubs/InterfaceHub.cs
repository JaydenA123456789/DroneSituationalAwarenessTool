using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using DroneSituationalAwarenessTool.Server.EntityStateFunctionality;
using SharedLibraries.EntityFunctionality;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DroneSituationalAwarenessTool.Server.SignalRHubs
{
    public class InterfaceHub : Hub
    {
        private readonly EntityState _entityState;

        public InterfaceHub(EntityState entityState)
        {
            _entityState = entityState;
        }

        public async Task SendMessage(string user, string message)
        {
            switch (user)
            {
                case "AirData":
                    try
                    {
                        _entityState.AddUpdateEntity(JsonConvert.DeserializeObject<AirEntity>(message));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error processing AirData: " + ex);
                    }
                    break;
                case "MaritimeData":
                    try
                    {
                        _entityState.AddUpdateEntity(JsonConvert.DeserializeObject<MaritimeEntity>(message));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error processing MaritimeData: " + ex);
                    }
                    break;
                case "DroneData":
                    try
                    {
                        _entityState.AddUpdateEntity(JsonConvert.DeserializeObject<DroneEntity>(message));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error processing DroneData: " + ex);
                    }
                    break;
                default:
                    Console.WriteLine("Unknown data type: " + user);
                    break;
            }
        }
    }
}
