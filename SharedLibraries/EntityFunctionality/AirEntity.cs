using SharedLibraries.HelperObjects;

namespace SharedLibraries.EntityFunctionality
{
    public class AirEntity : IMapEntity
    {
        public string Id { get; set; }
        public Position Position { get; set; }
        public Attitude? Attitude { get; set; }
        public DateTime Created_UTC { get; set; }
        public DateTime LastUpdate_UTC { get; set; }
        public DateTime? LastReported_UTC { get; set; }

        public List<Position> TracePositions { get; set; }
        public void UpdateEntity(IMapEntity newEntity)
        {
            //check that the newest point is above the threshold (vehicle has to move a certain distance before the trace point is recorded)
            double distance = TracePositions[TracePositions.Count-1].GetDistanceFrom(newEntity.Position);
            if (distance > 1)// meter/s 
            {
                //check that data isnt default 0,0,0 and it isnt a duplicate
                if (newEntity.Position.Latitude != 0 && newEntity.Position.Longitude != 0 &&
                    !TracePositions.Any(pos =>
                    pos.Latitude == newEntity.Position.Latitude &&
                    pos.Longitude == newEntity.Position.Longitude &&
                    pos.Altitude == newEntity.Position.Altitude))

                {
                    TracePositions.Add(newEntity.Position);
                }
            }

            Position = newEntity.Position;
            Attitude = newEntity.Attitude;
            LastUpdate_UTC = DateTime.Now;
            LastReported_UTC = newEntity.LastReported_UTC;
        }
    }
}
