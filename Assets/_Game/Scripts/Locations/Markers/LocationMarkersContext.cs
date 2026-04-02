using UnityEngine;

namespace Assets._Game.Scripts.Locations.Markers
{
    public sealed class LocationMarkersContext : MonoBehaviour
    {
        [field: SerializeField]
        public LocationEntranceMarker[] LocationEntranceMarkers { get; private set; }
        [field: SerializeField]
        public LocationTransitionMarker[] LocationTransitionMarkers { get; private set; }
        [field: SerializeField]
        public EntitySpawnPointMarker[] EntitySpawnPointMarkers { get; private set; }
        [field: SerializeField]
        public EntitySpawnSpotMarker[] EntitySpawnSpotMarkers { get; private set; }
    }
}
