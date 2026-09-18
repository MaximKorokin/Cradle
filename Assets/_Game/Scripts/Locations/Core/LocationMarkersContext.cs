using Assets._Game.Scripts.Locations.Markers;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Locations.Core
{
    public sealed class LocationMarkersContext : MonoBehaviour
    {
        [SerializeField]
        private bool _autoLoadChildrenMarkers = true;

        [field: SerializeField]
        public LocationEntranceMarker[] LocationEntranceMarkers { get; private set; }
        [field: SerializeField]
        public LocationTransitionMarker[] LocationTransitionMarkers { get; private set; }
        [field: SerializeField]
        public EntitySpawnSpotMarker[] EntitySpawnSpotMarkers { get; private set; }

        private EntitySpawnSpotRuntime[] _entitySpawnSpotRuntimes;

        public EntitySpawnSpotRuntime[] GetEntitySpawnSpotRuntimes()
        {
            LoadMarkers();
            if (_entitySpawnSpotRuntimes != null) return _entitySpawnSpotRuntimes;

            _entitySpawnSpotRuntimes = new EntitySpawnSpotRuntime[EntitySpawnSpotMarkers.Length];
            for (int i = 0; i < EntitySpawnSpotMarkers.Length; i++)
            {
                _entitySpawnSpotRuntimes[i] = EntitySpawnSpotMarkers[i].Definition.CreateRuntime(EntitySpawnSpotMarkers[i].transform.position);
            }
            return _entitySpawnSpotRuntimes;
        }

        private void LoadMarkers()
        {
            if (_autoLoadChildrenMarkers)
            {
                LocationEntranceMarkers = LocationEntranceMarkers.Union(GetComponentsInChildren<LocationEntranceMarker>(false)).ToArray();
                LocationTransitionMarkers = LocationTransitionMarkers.Union(GetComponentsInChildren<LocationTransitionMarker>(false)).ToArray();
                EntitySpawnSpotMarkers = EntitySpawnSpotMarkers.Union(GetComponentsInChildren<EntitySpawnSpotMarker>(false)).ToArray();
            }
        }
    }
}
