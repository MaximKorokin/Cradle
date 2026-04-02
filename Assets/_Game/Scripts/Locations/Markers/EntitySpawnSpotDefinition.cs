using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Storage;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Locations.Markers
{
    [CreateAssetMenu(menuName = "Game/EntitySpawnSpotDefinition")]
    public class EntitySpawnSpotDefinition : GuidScriptableObject
    {
        [field: SerializeField]
        public SpawnGroupDefinition[] Groups { get; private set; }
        [field: SerializeField]
        public float Radius { get; private set; }

        [field: SerializeField]
        public bool SpawnOnLocationLoad { get; private set; } = true;
    }

    [Serializable]
    public struct SpawnGroupDefinition
    {
        public EntityDefinition EntityDefinition;
        public int Count;
    }
}
