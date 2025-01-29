using SharedLibraries.EntityFunctionality;

namespace DroneSituationalAwarenessTool.Server.EntityStateFunctionality
{
    public interface IEntityState
    {
        void AddUpdateEntity(Debug_GenericEntity newEntity);
        void AddUpdateEntity(AirEntity newEntity);
        void AddUpdateEntity(DroneEntity newEntity);
        void AddUpdateEntity(MaritimeEntity newEntity);
    }
}