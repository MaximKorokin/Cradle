using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Storage;
using UnityEngine;

namespace Assets._Game.Scripts.Locations.Markers
{
    [CreateAssetMenu(menuName = "Game/EntitySpawnPointDefinition")]
    public sealed class EntitySpawnPointDefinition : GuidScriptableObject
    {
        [field: SerializeField]
        public EntityDefinition EntityDefinition { get; private set; }

        [field: SerializeField]
        public bool SpawnOnLocationLoad { get; private set; } = true;
    }
}
