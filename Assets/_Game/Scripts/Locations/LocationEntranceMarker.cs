using UnityEngine;

namespace Assets._Game.Scripts.Locations
{
    public sealed class LocationEntranceMarker : MonoBehaviour
    {
        [field: SerializeField]
        public string EntranceId { get; private set; }

        [field: SerializeField]
        public Transform SpawnPoint { get; private set; }

        [field: SerializeField]
        public bool FaceRight { get; private set; } = true;

        public Vector3 Position => SpawnPoint != null ? SpawnPoint.position : transform.position;
    }
}
