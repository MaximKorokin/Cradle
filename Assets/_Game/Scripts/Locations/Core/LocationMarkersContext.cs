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
    }
}
