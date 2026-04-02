using UnityEngine;

namespace Assets._Game.Scripts.Locations.Markers
{
    public sealed class EntitySpawnPointMarker : MonoBehaviour
    {
        [field: SerializeField]
        public EntitySpawnPointDefinition Definition { get; private set; }
    }
}
