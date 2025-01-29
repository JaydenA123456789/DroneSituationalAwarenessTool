using SharedLibraries.HelperObjects;

namespace SharedLibraries.EntityFunctionality
{
    public class Debug_GenericEntity : IMapEntity
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
            Position = newEntity.Position;
            Attitude = newEntity.Attitude;
            LastUpdate_UTC = DateTime.Now;
            LastReported_UTC = newEntity.LastReported_UTC;
            TracePositions.Append(newEntity.Position);
        }

    }
}
