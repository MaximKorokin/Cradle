using Assets._Game.Scripts.Entities;
using UnityEngine;

namespace Assets._Game.Scripts.Locations
{
    public sealed class EntitySpawnMarker : MonoBehaviour
    {
        [field: SerializeField]
        public EntityDefinition Definition { get; private set; }

        [field: SerializeField]
        public bool SpawnOnLocationLoad { get; private set; } = true;
    }
}
