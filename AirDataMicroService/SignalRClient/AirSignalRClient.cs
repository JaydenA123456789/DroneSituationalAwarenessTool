using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using SharedLibraries.EntityFunctionality;
using SharedLibraries.HelperObjects;
using System.Net;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AirDataMicroService.AirSignalRClient
{
    public class AirSignalRClient : ISignalRClient
    {
        
        public AirSignalRClient()
        {
            System.Threading.Thread.Sleep(1000);
            StartConnection();
        }
        private int airPacketNumber = 0;
        public async void StartConnection()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7017/InterfaceHub")
                .Build();

            await connection.StartAsync();
            Console.WriteLine("Connected to SignalR hub.");

            while (true)
            {
                await Task.Delay(2500);

                var handler = new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                using var client = new HttpClient(handler);
                var url = "https://tx.ozrunways.com/tx/geo";

                dynamic jsonObj = null;
                try {
                    string jsonString = await client.GetStringAsync(url);
                    jsonObj = JsonConvert.DeserializeObject(jsonString);
                } catch {
                    Console.WriteLine("Error getting data or converting to JSON format");
                    continue;
                }
                try
                {
                    if (jsonObj != null)
                    {

                        foreach (var aircraft in jsonObj.features)
                        {                        

                            Console.WriteLine($"Air data number: {airPacketNumber++}");

                            string regexPattern = @"(\d{1,3}(,\d{3})?) ft<";
                            string regexInput = aircraft.properties.popupContent;
                            double altitude = 0;//default value in case of error
                            Match match = Regex.Match(regexInput, regexPattern);
                            if (match.Success)
                            {
                                altitude = Convert.ToDouble((match.Groups[1].Value).Replace(",", ""));
                            }

                            AirEntity newEntity = new AirEntity
                            {
                                Id = aircraft.properties.tk.ToString(),
                                Position = new Position(
                                    (double)aircraft.geometry.coordinates[1],
                                    (double)aircraft.geometry.coordinates[0],
                                    altitude
                                    ),
                                Attitude = new Attitude(0.0, 0.0, (double)aircraft.properties.trk),
                                Created_UTC = DateTime.Now,
                                LastUpdate_UTC = DateTime.Now,
                                LastReported_UTC = DateTime.Now,
                                TracePositions = new List<Position>
                                {
                                    new Position(
                                        (double)aircraft.geometry.coordinates[1],
                                        (double)aircraft.geometry.coordinates[0],
                                        altitude
                                    )
                                }
                            };

                            await connection.InvokeAsync("SendMessage", "AirData", JsonConvert.SerializeObject(newEntity));
                            Console.WriteLine("sent message");
                        }
                        Console.WriteLine("Messages sent to server.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send message: {ex.Message}");
                }
            }
        }
    }
} 

