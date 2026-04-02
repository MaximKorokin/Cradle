using Assets._Game.Scripts.Locations.Markers;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class LocationEntitySpawnSystem : SystemBase, IStartSystem
    {
        private readonly LocationMarkersContext _locationMarkersContext;

        public LocationEntitySpawnSystem(LocationMarkersContext locationMarkersContext)
        {
            _locationMarkersContext = locationMarkersContext;
        }

        public void Start()
        {

        }
    }
}
