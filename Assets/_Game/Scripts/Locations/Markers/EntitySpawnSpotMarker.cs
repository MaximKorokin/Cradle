using UnityEngine;

namespace Assets._Game.Scripts.Locations.Markers
{
    public sealed class EntitySpawnSpotMarker : MonoBehaviour
    {
        [field: SerializeField]
        public EntitySpawnSpotDefinition Definition { get; private set; }
    }
}
