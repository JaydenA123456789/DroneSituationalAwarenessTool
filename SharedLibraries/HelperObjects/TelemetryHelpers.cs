using ProjNet;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace SharedLibraries.HelperObjects
{
    public class Position
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }

        public Position(double latitude, double longitude, double altitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        public double GetDistanceFrom(Position comparePosition) {
            double R = 6371000; // Earth radius in meters
            double lat1Rad = this.Latitude * Math.PI / 180.0;
            double lat2Rad = comparePosition.Latitude * Math.PI / 180.0;
            double deltaLat = (comparePosition.Latitude - this.Latitude) * Math.PI / 180.0;
            double deltaLon = (comparePosition.Longitude - this.Longitude) * Math.PI / 180.0;

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // Distance in meters
        }
    }

    public class Attitude
    {
        public double Roll { get; set; }
        public double Pitch { get; set; }//0-horizon, 90-up, 180-horizon, 270-down
        public double Yaw { get; set; } //heading from 0/360 degrees(North)

        public Attitude(double roll, double pitch, double yaw)
        {
            Roll = roll;
            Pitch = pitch;
            Yaw = yaw;
        }
    }

}
