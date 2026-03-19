using Assets._Game.Scripts.Infrastructure.Configs;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class DespawnModule : EntityModuleBase
    {
        public DespawnCounterStartTrigger Trigger { get; set; }

        public float? StartDespawnTime { get; set; } = null;
        public float DespawnDelay { get; }

        public bool IsExpired => StartDespawnTime != null && (Time.time - StartDespawnTime.Value > DespawnDelay);

        public DespawnModule(float despawnDelay, DespawnCounterStartTrigger trigger)
        {
            DespawnDelay = despawnDelay;
            Trigger = trigger;
        }
    }

    public sealed class DespawnModuleFactory : IEntityModuleFactory
    {
        private readonly DespawnConfig _config;

        public DespawnModuleFactory(DespawnConfig despawnConfig)
        {
            _config = despawnConfig;
        }

        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (entityDefinition.TryGetModuleDefinition<DespawnModuleDefinition>(out var definition))
            {
                var time = Mathf.Clamp(definition.DespawnDelay, _config.MinDespawnDelay, _config.MaxDespawnDelay);
                return new DespawnModule(time, definition.Trigger);
            }
            return null;
        }
    }
}
