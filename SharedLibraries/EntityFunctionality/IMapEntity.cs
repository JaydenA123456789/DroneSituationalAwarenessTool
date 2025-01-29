using SharedLibraries.HelperObjects;

namespace SharedLibraries.EntityFunctionality
{
    public interface IMapEntity
    {
        public string Id { get; set; }
        //Positional
        public Position Position { get; set; }
        //Attitude
        public Attitude? Attitude { get; set; }
        //Time based data
        public  DateTime Created_UTC { get; set; }
        public DateTime LastUpdate_UTC { get; set; }
        public DateTime? LastReported_UTC { get; set; }
        public List<Position> TracePositions { get; set; }

        public void UpdateEntity(IMapEntity entity);
    }
}
