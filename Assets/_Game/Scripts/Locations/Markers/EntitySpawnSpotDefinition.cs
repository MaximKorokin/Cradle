using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Storage;
using Assets.CoreScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Locations.Markers
{
    [CreateAssetMenu(menuName = "Game/EntitySpawnSpotDefinition")]
    public class EntitySpawnSpotDefinition : GuidScriptableObject
    {
        [field: SerializeField]
        public EntitySpawnEntry[] Entities { get; private set; }
        [field: SerializeField]
        public float Radius { get; private set; }
        [field: SerializeField]
        public bool RespawnEnabled { get; private set; } = true;

        public EntitySpawnSpotRuntime CreateRuntime(Vector2 center)
        {
            return new(this, center);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            var hashSet = new HashSet<string>();
            for (int i = 0; i < Entities.Length; i++)
            {
                var entityDefinition = Entities[i].EntityDefinition;
                if (entityDefinition == null)
                {
                    SLog.Error($"EntitySpawnSpotDefinition {name} has a null EntityDefinition at index {i} in its Entities array.");
                }
                else if (!hashSet.Add(entityDefinition.Id))
                {
                    SLog.Error($"EntitySpawnSpotDefinition {name} has duplicate EntityDefinition '{entityDefinition.name}' at index {i} in its Entities array.");
                }
            }
        }
    }

    [Serializable]
    public struct EntitySpawnEntry
    {
        public EntityDefinition EntityDefinition;
        public float RespawnTime;
        public int Count;
    }

    public sealed class EntitySpawnSpotRuntime
    {
        private readonly EntitySpawnSpotDefinition _definition;
        private readonly Dictionary<string, (CooldownCounter Cooldown, bool IsSpawned)[]> _respawnCooldowns;

        public string Id { get; } = Guid.NewGuid().ToString();
        public IReadOnlyList<EntityDefinition> EntityDefinitions { get; }
        public Vector3 Center { get; }
        public float Radius => _definition.Radius;

        public EntitySpawnSpotRuntime(EntitySpawnSpotDefinition definition, Vector3 center)
        {
            _definition = definition;
            EntityDefinitions = _definition.Entities.Select(e => e.EntityDefinition).ToList();
            Center = center;

            _respawnCooldowns = new();
            for (int i = 0; i < _definition.Entities.Length; i++)
            {
                var entry = _definition.Entities[i];
                _respawnCooldowns[entry.EntityDefinition.Id] = new (CooldownCounter Cooldown, bool IsSpawned)[entry.Count];
                for (int j = 0; j < entry.Count; j++)
                {
                    _respawnCooldowns[entry.EntityDefinition.Id][j] = (new CooldownCounter(entry.RespawnTime), false);
                }
            }
        }

        public int GetSpawnAmount(string entityDefinitionId)
        {
            if (!_definition.RespawnEnabled) return 0;
            if (!ValidateEntityDefinitionId(entityDefinitionId)) return 0;

            var cooldowns = _respawnCooldowns[entityDefinitionId];
            var spawnAmount = 0;

            for (int i = 0; i < cooldowns.Length; i++)
            {
                if (!cooldowns[i].IsSpawned && cooldowns[i].Cooldown.IsOver())
                    spawnAmount++;
            }

            return spawnAmount;
        }

        public void MarkSpawned(string entityDefinitionId)
        {
            if (!ValidateEntityDefinitionId(entityDefinitionId)) return;

            var cooldowns = _respawnCooldowns[entityDefinitionId];

            for (int i = 0; i < cooldowns.Length; i++)
            {
                if (!cooldowns[i].IsSpawned)
                {
                    cooldowns[i].IsSpawned = true;
                    return;
                }
            }

            SLog.Error($"EntitySpawnSpotRuntime {_definition.name} with ID '{Id}' cannot mark EntityDefinition with ID '{entityDefinitionId}' as spawned because there are no available spawn slots.");
        }

        public void MarkDespawned(string entityDefinitionId)
        {
            if (!ValidateEntityDefinitionId(entityDefinitionId)) return;

            var cooldowns = _respawnCooldowns[entityDefinitionId];

            for (int i = 0; i < cooldowns.Length; i++)
            {
                if (cooldowns[i].IsSpawned)
                {
                    cooldowns[i].Cooldown.Reset();
                    cooldowns[i].IsSpawned = false;
                    return;
                }
            }

            SLog.Error($"EntitySpawnSpotRuntime {_definition.name} with ID '{Id}' cannot mark EntityDefinition with ID '{entityDefinitionId}' as despawned because there are no currently spawned entities of this definition.");
        }

        private bool ValidateEntityDefinitionId(string entityDefinitionId)
        {
            if (!_respawnCooldowns.ContainsKey(entityDefinitionId))
            {
                SLog.Error($"EntitySpawnSpotRuntime for '{_definition.name}' does not contain an entry for EntityDefinition with ID '{entityDefinitionId}'.");
                return false;
            }
            return true;
        }
    }
}
