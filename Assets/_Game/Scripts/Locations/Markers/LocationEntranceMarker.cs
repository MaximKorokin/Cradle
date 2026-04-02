using UnityEngine;

namespace Assets._Game.Scripts.Locations.Markers
{
    public sealed class LocationEntranceMarker : MonoBehaviour
    {
        [field: SerializeField]
        public string EntranceId { get; private set; }

        [field: SerializeField]
        public Transform SpawnPoint { get; private set; }

        public Vector3 Position => SpawnPoint != null ? SpawnPoint.position : transform.position;
    }
}
