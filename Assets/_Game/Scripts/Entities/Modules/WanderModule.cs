using UnityEngine;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class WanderModule : EntityModuleBase
    {
        public readonly float MinIdleTime;
        public readonly float MaxIdleTime;

        public Vector2? AnchorPoint { get; set; }
        public Vector2? CurrentPoint { get; set; }

        public WanderModule(float minIdleTime, float maxIdleTime)
        {
            MinIdleTime = minIdleTime;
            MaxIdleTime = maxIdleTime;
        }
    }

    public sealed class WanderModuleFactory
    {
        public WanderModuleFactory()
        {
        }

        public WanderModule Create(EntityDefinition entityDefinition)
        {
            if (entityDefinition.TryGetModuleDefinition<WanderModuleDefinition>(out var wanderModuleDefinition))
            {
                return new WanderModule(wanderModuleDefinition.MinIdleTime, wanderModuleDefinition.MaxIdleTime);
            }
            return null;
        }
    }
}
