using Microsoft.AspNetCore.SignalR;
using DroneSituationalAwarenessTool.Server.SignalRHubs;
using SharedLibraries.EntityFunctionality;
using Newtonsoft.Json;
using SharedLibraries.HelperObjects;

namespace DroneSituationalAwarenessTool.Server.EntityStateFunctionality
{
    public class EntityState : IEntityState
    {
        public DateTime CreationTime { get; set; }
        public List<Debug_GenericEntity> Debug_GenericEntityList { get; private set; }
        public List<DroneEntity> DroneEntityList { get; private set; }
        public List<AirEntity> AirEntityList { get; private set; }
        public List<MaritimeEntity> MaritimeEntityList { get; private set; }

        //array locks for threadsafe array access
        private static readonly ReaderWriterLockSlim _DebugGenericLock = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim _DroneEntityLock = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim _AirEntityLock = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim _MaritimeEntityLock = new ReaderWriterLockSlim();

        private readonly IHubContext<ClientHub> _clientHubContext;

        public EntityState(IHubContext<ClientHub> clientHubContext)
        {
            _clientHubContext = clientHubContext;

            CreationTime = DateTime.Now;

            Debug_GenericEntityList = new List<Debug_GenericEntity>();
            DroneEntityList = new List<DroneEntity>();
            AirEntityList = new List<AirEntity>();
            MaritimeEntityList = new List<MaritimeEntity>();

            //Add dummy drone so that it will render air and maritime data without an active drone
            new DroneEntity
            {
                Id = "Drone_Demo_Dummy",
                Position = new Position( //In Brisbane
                -27.47,
                153.0,
                -100000.0
                ),
                Attitude = new Attitude(0.0, 0.0, 0.0),
                Created_UTC = DateTime.Now,
                LastUpdate_UTC = DateTime.Now,
                LastReported_UTC = DateTime.Now,
                TracePositions = new List<Position>()
            };

            CreateStaleRunner(0.5);//create task to update time (hz)
        }
        public void AddUpdateEntity(Debug_GenericEntity newEntity)
        {
            _DebugGenericLock.EnterWriteLock();
            foreach (Debug_GenericEntity arrayEntity in Debug_GenericEntityList)
            {
                if (newEntity.Id == arrayEntity.Id)
                {
                    arrayEntity.UpdateEntity(newEntity);
                    SendEntityUpdateToClient(arrayEntity);
                    _DebugGenericLock.ExitWriteLock();
                    return;
                }
            }
            Debug_GenericEntityList.Add(newEntity);
            SendEntityUpdateToClient(newEntity);
            _DebugGenericLock.ExitWriteLock();
            return;
        }

        public void AddUpdateEntity(AirEntity newEntity)
        {
            try
            {
                _AirEntityLock.EnterWriteLock();
                if (OutSideRenderDist(newEntity, 2))
                {
                    _AirEntityLock.ExitWriteLock();
                    return;
                }
                foreach (AirEntity arrayEntity in AirEntityList)
                {
                    if (newEntity.Id == arrayEntity.Id)
                    {
                        arrayEntity.UpdateEntity(newEntity);
                        SendEntityUpdateToClient(arrayEntity);
                        SendTrackUpdateToClient(arrayEntity);
                        _AirEntityLock.ExitWriteLock();
                        return;
                    }
                }
                AirEntityList.Add(newEntity);
                SendEntityUpdateToClient(newEntity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            _AirEntityLock.ExitWriteLock();
            return;
        }

        public void AddUpdateEntity(MaritimeEntity newEntity)
        {
            _MaritimeEntityLock.EnterWriteLock();
            if (OutSideRenderDist(newEntity, 0.5)) {
                _MaritimeEntityLock.ExitWriteLock();
                return;
            }
            foreach (MaritimeEntity arrayEntity in MaritimeEntityList)
            {
                if (newEntity.Id == arrayEntity.Id)
                {
                    arrayEntity.UpdateEntity(newEntity);
                    SendEntityUpdateToClient(arrayEntity);
                    SendTrackUpdateToClient(arrayEntity);
                    _MaritimeEntityLock.ExitWriteLock();
                    return;
                }
            }
            if (MaritimeEntityList.Count > 100) { _MaritimeEntityLock.ExitWriteLock(); return; }; //Reduce render count due to hardware limitiations (need to test with higher counts)
            MaritimeEntityList.Add(newEntity);
            SendEntityUpdateToClient(newEntity);
            _MaritimeEntityLock.ExitWriteLock();
            return;
        }

        public void AddUpdateEntity(DroneEntity newEntity)
        {
            _DroneEntityLock.EnterWriteLock();
            foreach (DroneEntity arrayEntity in DroneEntityList)
            {
                if (newEntity.Id == arrayEntity.Id)
                {
                    arrayEntity.UpdateEntity(newEntity);
                    SendEntityUpdateToClient(arrayEntity);
                    SendTrackUpdateToClient(arrayEntity);
                    _DroneEntityLock.ExitWriteLock();
                    return;
                }
            }
            DroneEntityList.Add(newEntity);
            SendEntityUpdateToClient(newEntity);
            _DroneEntityLock.ExitWriteLock();
            return;
        }

        private bool OutSideRenderDist(IMapEntity mapEntity, double deg_maxRenderDist)
        {
            _DroneEntityLock.EnterWriteLock();
            foreach (DroneEntity drone in DroneEntityList)
            {
                // Calculate distance between mapEntity and drone
                double deg_pointDist = Math.Sqrt(
                    Math.Pow(mapEntity.Position.Longitude - drone.Position.Longitude, 2) +
                    Math.Pow(mapEntity.Position.Latitude - drone.Position.Latitude, 2)
                );

                if (deg_pointDist < deg_maxRenderDist)
                {
                    _DroneEntityLock.ExitWriteLock();
                    return false; // mapEntity is not outside render distance of at least one drone
                }
            }
            _DroneEntityLock.ExitWriteLock();
            return true; // mapEntity is outside render distance of all drones
        }

        private void SendTrackUpdateToClient(IMapEntity entity)
        {
            try
            {
                _clientHubContext.Clients.All.SendAsync("UpdateTrackToCesium", entity.GetType().ToString(), JsonConvert.SerializeObject(entity));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        private void SendEntityUpdateToClient(IMapEntity entity)
        {
            try
            {
                _clientHubContext.Clients.All.SendAsync("UpdateAddToCesium", entity.GetType().ToString(), JsonConvert.SerializeObject(entity));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }
        private void Stale_SendUpdateToClient(IMapEntity entity)
        {
            try
            {
                _clientHubContext.Clients.All.SendAsync("UpdateAddToCesium", "Stale", JsonConvert.SerializeObject(entity));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        private Task DeleteFromArray(IMapEntity entity)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Deleting from array: " + entity.ToString());

                switch (entity)
                {
                    case AirEntity airEntity:
                        _AirEntityLock.EnterWriteLock();
                        try
                        {
                            AirEntityList.Remove(airEntity);
                        }
                        finally
                        {
                            _AirEntityLock.ExitWriteLock();
                        }
                        break;

                    case MaritimeEntity maritimeEntity:
                        _MaritimeEntityLock.EnterWriteLock();
                        try
                        {
                            MaritimeEntityList.Remove(maritimeEntity);
                        }
                        finally
                        {
                            _MaritimeEntityLock.ExitWriteLock();
                        }
                        break;

                    case DroneEntity droneEntity:
                        _DroneEntityLock.EnterWriteLock();
                        try
                        {
                            DroneEntityList.Remove(droneEntity);
                        }
                        finally
                        {
                            _DroneEntityLock.ExitWriteLock();
                        }
                        break;

                    default:
                        Console.WriteLine("Cannot remove this type");
                        break;
                }
                Console.WriteLine("Deleted");
            });
        }

        private void RemoveEntityAndTrack(IMapEntity entity)
        {
            try
            {
                _clientHubContext.Clients.All.SendAsync("DeleteFromCesium", "EntityAndTrack", entity.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        private void CheckStale(IMapEntity entity) 
        {
            DateTime currentUtcTime = DateTime.Now;
            DateTime lastReportedUtcTime = (DateTime)entity.LastReported_UTC; // Assume this is a DateTime

            // Calculate the time difference
            TimeSpan timeDifference = currentUtcTime - lastReportedUtcTime;
            double int_timeDifference = timeDifference.TotalMilliseconds;

            //If x seconds no data then delete
            if (timeDifference.TotalMilliseconds > 60000)
            {
                try 
                {
                    RemoveEntityAndTrack(entity);
                    DeleteFromArray(entity); 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Task encountered an error and stopped: {ex.Message}");
                    Console.WriteLine($"StackTrace: {ex.StackTrace}");
                }

            } //If x seconds no data then make stale (grey spheres)
            else if (timeDifference.TotalMilliseconds > 20000 )
            {
                Stale_SendUpdateToClient(entity);
            }
        }

        private async void CreateStaleRunner(double updateRateHz)
        {
            int delayRateMS = (int)Math.Ceiling((1.0 / updateRateHz) * 1000);
            try
            {
                await Task.Run(async () =>
                {
                    while (true)
                    {
                        await Task.Delay(delayRateMS);

                        _AirEntityLock.EnterWriteLock();
                        foreach (IMapEntity entity in AirEntityList)
                        {
                            CheckStale(entity);
                        }
                        _AirEntityLock.ExitWriteLock();
                        _MaritimeEntityLock.EnterWriteLock();
                        foreach (IMapEntity entity in MaritimeEntityList)
                        {
                            CheckStale(entity);
                        }
                        _MaritimeEntityLock.ExitWriteLock();
                        _DroneEntityLock.EnterWriteLock();
                        foreach (IMapEntity entity in DroneEntityList)
                        {
                            CheckStale(entity);
                        }
                        _DroneEntityLock.ExitWriteLock();
                    }
                });
            }
            catch (Exception ex) {
                Console.WriteLine("exception "+ ex);
                try
                {
                    //Release locks if owned to make exception non-blocking
                    if(_AirEntityLock.IsWriteLockHeld) _AirEntityLock.ExitWriteLock();
                    if (_MaritimeEntityLock.IsWriteLockHeld) _MaritimeEntityLock.ExitWriteLock();
                    if (_DroneEntityLock.IsWriteLockHeld) _DroneEntityLock.ExitWriteLock();
                }
                catch (Exception ex2) {
                    Console.WriteLine("Backup runner fail: "+ex2);
                }
                CreateStaleRunner(updateRateHz); //Restart stale checker
            }
        }
    }
}
