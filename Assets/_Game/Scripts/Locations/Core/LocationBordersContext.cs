using UnityEngine;

namespace Assets._Game.Scripts.Locations.Core
{
    public sealed class LocationBordersContext : MonoBehaviour
    {
        [field: SerializeField]
        public PolygonCollider2D CameraBorder { get; private set; }
    }
}
