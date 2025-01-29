using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using SharedLibraries.EntityFunctionality;
using SharedLibraries.HelperObjects;
using System.Net.Sockets;
using System.Net;
using static MAVLink;


namespace MavLinkMicroService.MavLinkSignalRClient
{
    public class MavLinkSignalRClient : ISignalRClient
    {
        //create object to populate with data
        DroneEntity newEntity = new DroneEntity
        {
            Id = "Drone1", //Hardcoded for now, but you could get the systemID and assign it here and it would work for multiple vehicles
            Position = new Position(
                0.0,
                0.0,
                0.0
                ),
            Attitude = new Attitude(0.0, 0.0, 0.0),
            Created_UTC = DateTime.Now,
            LastUpdate_UTC = DateTime.Now,
            LastReported_UTC = DateTime.Now,
            TracePositions = new List<Position>()
        };

        public MavLinkSignalRClient()
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
                Console.WriteLine($"Listening for Mavlink data.");

                string host = "127.0.0.1";
                int port = 4112;
                TcpListener listener = new TcpListener(IPAddress.Parse(host), port);
                listener.Start();

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Client connected.");

                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead;

                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            string data = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            SendData(stream, connection);
                        }
                    }

                    client.Close();
                    Console.WriteLine("Client disconnected.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

        }

        private int number =0;
        private async void SendData(NetworkStream stream, HubConnection connection)
        {
           var mavlink = new MAVLink(); // Create a Mavlink parser
            //mavlink.Version = MAVLINK_VERSION.MAVLINK_10;

            byte[] buffer = new byte[1024];
            while (true)
            {
                try
                {
                    // Read data from the TCP stream
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    var mavlinkMessage = new MAVLink.MAVLinkMessage();
                    mavlinkMessage.buffer = buffer;
                    Console.WriteLine(mavlinkMessage.ToString());
                    string packet = mavlinkMessage.ToString();
                    dynamic jsonObj = null;
                    try
                    {
                        if (mavlinkMessage.msgtypename == "ATTITUDE") 
                        {
                            var attitudeData = (mavlink_attitude_t)mavlinkMessage.data;

                            double Roll = Convert.ToDouble(attitudeData.roll * (180.0 / Math.PI));
                            double Pitch = Convert.ToDouble(attitudeData.pitch * (180.0 / Math.PI));
                            double Yaw = Convert.ToDouble(attitudeData.yaw * (180.0 / Math.PI));

                            newEntity.Attitude = new Attitude(Roll, Pitch, Yaw);
                            newEntity.Created_UTC = DateTime.Now;
                            newEntity.LastUpdate_UTC = DateTime.Now;
                            newEntity.LastReported_UTC = DateTime.Now;

                            await connection.InvokeAsync("SendMessage", "DroneData", JsonConvert.SerializeObject(newEntity));
                        }
                        if (mavlinkMessage.msgtypename == "GLOBAL_POSITION_INT")
                        {
                            var positionData = (mavlink_global_position_int_t)mavlinkMessage.data;

                            double Latitude = Convert.ToDouble(positionData.lat) / 1e7;
                            double Longitude = Convert.ToDouble(positionData.lon) / 1e7;
                            double Altitude= Convert.ToDouble(positionData.alt) / 1e3;

                            newEntity.Position = new Position(Latitude, Longitude, Altitude);
                            newEntity.Created_UTC = DateTime.Now;
                            newEntity.LastUpdate_UTC = DateTime.Now;
                            newEntity.LastReported_UTC = DateTime.Now;

                            if (newEntity.Position.Latitude == 0 || newEntity.Position.Longitude == 0) return;
                            await connection.InvokeAsync("SendMessage", "DroneData", JsonConvert.SerializeObject(newEntity));
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Error getting data or converting to JSON format");
                        return;
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Connection closed.");
                    break;
                }
            }
        }
    }
}