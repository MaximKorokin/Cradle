using Assets._Game.Scripts.Locations.Markers;
using UnityEngine;

namespace Assets._Game.Scripts.Locations.Core
{
    public sealed class LocationMarkersContext : MonoBehaviour
    {
        [field: SerializeField]
        public LocationEntranceMarker[] LocationEntranceMarkers { get; private set; }
        [field: SerializeField]
        public LocationTransitionMarker[] LocationTransitionMarkers { get; private set; }
        [field: SerializeField]
        public EntitySpawnSpotMarker[] EntitySpawnSpotMarkers { get; private set; }

        private EntitySpawnSpotRuntime[] _entitySpawnSpotRuntimes;

        public EntitySpawnSpotRuntime[] GetEntitySpawnSpotRuntimes()
        {
            if (_entitySpawnSpotRuntimes != null) return _entitySpawnSpotRuntimes;

            _entitySpawnSpotRuntimes = new EntitySpawnSpotRuntime[EntitySpawnSpotMarkers.Length];
            for (int i = 0; i < EntitySpawnSpotMarkers.Length; i++)
            {
                _entitySpawnSpotRuntimes[i] = EntitySpawnSpotMarkers[i].Definition.CreateRuntime(EntitySpawnSpotMarkers[i].transform.position);
            }
            return _entitySpawnSpotRuntimes;
        }
    }
}
