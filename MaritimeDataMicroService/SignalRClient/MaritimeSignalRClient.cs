using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedLibraries.EntityFunctionality;
using SharedLibraries.HelperObjects;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;

namespace MaritimeDataMicroService.MaritimeSignalRClient
{
    public class MaritimeSignalRClient : ISignalRClient
    {
        public MaritimeSignalRClient()
        {
            System.Threading.Thread.Sleep(1000);
            StartConnection();
        }
        public async void StartConnection()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7017/InterfaceHub")
                .Build();

            await connection.StartAsync();
            Console.WriteLine("Connected to SignalR hub.");
            try
            {
                using (var clientWebSocket = new ClientWebSocket())
                {
                    var webSocketUri = new Uri("wss://stream.aisstream.io/v0/stream");
                    Console.WriteLine("Connecting to WebSocket...");
                    await clientWebSocket.ConnectAsync(webSocketUri, CancellationToken.None);

                    Console.WriteLine("Connected to WebSocket!");

                    // Subscription message
                    var message = @"
                        {
                            ""APIKey"": ""e9f5590c585a73715c98cd1f1561bc7456408081"",
                            ""BoundingBoxes"": [
                                [

                                    [-25, 149], [-28, 154]

                                ]
                            ]
                        }";
                    /*   
                        [49.384358, -124.848974],
                        [24.396308, -124.848974],
                        [49.384358, -66.93457],
                        [24.396308, -66.93457]
                     */

                    var messageBytes = Encoding.UTF8.GetBytes(message);
                    await clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    Console.WriteLine("Message sent to server.");

                    var random = new Random();
                    int packetNumber = 0;
                    while (true)
                    {
                        // Listening for incoming messages
                        var buffer = new byte[1024 * 4];
                        while (clientWebSocket.State == WebSocketState.Open)
                        {
                            var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                Console.WriteLine("WebSocket closed by server.");
                                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                            }
                            else
                            {
                                var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                                Console.WriteLine($"Maritime Data Packet no.: {packetNumber}");
                                //if ((packetNumber++ < 500) || (random.Next(3) == 0)) // 1 in 3 chance after 500 updates
                                if (true)
                                {
                                    SendData(connection, receivedMessage);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Maritime SR Exception: {ex.Message}");
            }

        }

        private int number =0;
        private async void SendData(HubConnection connection, string jsonString) {
            Console.WriteLine($"maratime number{number++}");
            dynamic jsonObj = null;
            try
            {
                jsonObj = JsonConvert.DeserializeObject(jsonString);
            }
            catch
            {
                Console.WriteLine("Error getting data or converting to JSON format");
                return;
            }
            string JSONMessageType = jsonObj.MessageType;
            double TrueHeading = 0.0;
            try
            {
                if (!string.IsNullOrEmpty(JSONMessageType) && jsonObj["Message"] is JObject messageObject)
                {
                    // Get the correct message section dynamically
                    var specificMessage = messageObject[JSONMessageType];

                    if (specificMessage != null && specificMessage["TrueHeading"] != null)
                    {
                        double.TryParse(specificMessage["TrueHeading"].ToString(), out TrueHeading);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No TrueHeading: {ex.Message}");
            }
            if (jsonObj != null)
            {              
                try
                {
                    MaritimeEntity newEntity = new MaritimeEntity
                    {
                        Id = jsonObj.MetaData.MMSI,
                        Position = new Position(
                            Convert.ToDouble(jsonObj.MetaData.latitude),
                            Convert.ToDouble(jsonObj.MetaData.longitude),
                            20
                            ),
                        Attitude = new Attitude(0.0, 0.0, TrueHeading),
                        Created_UTC = DateTime.Now,
                        LastUpdate_UTC = DateTime.Now,
                        LastReported_UTC = DateTime.Now,
                        TracePositions = new List<Position>
                        {
                            new Position(
                                Convert.ToDouble(jsonObj.MetaData.latitude),
                                Convert.ToDouble(jsonObj.MetaData.longitude),
                                20
                                )
                        }
                    };

                    await connection.InvokeAsync("SendMessage", "MaritimeData", JsonConvert.SerializeObject(newEntity));
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send message: {ex.Message}");
                }
            }
            
        }
    }
} 