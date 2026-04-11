using UnityEngine;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class WanderBehaviourModule : EntityModuleBase
    {
        public readonly float MinIdleTime;
        public readonly float MaxIdleTime;
        public readonly float WanderRadius;

        public Vector2? AnchorPoint { get; set; }
        public Vector2? CurrentPoint { get; set; }

        public WanderBehaviourModule(float minIdleTime, float maxIdleTime, float wanderRadius)
        {
            MinIdleTime = minIdleTime;
            MaxIdleTime = maxIdleTime;
            WanderRadius = wanderRadius;
        }
    }

    public sealed class WanderModuleFactory : IEntityModuleFactory
    {
        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (entityDefinition.TryGetModuleDefinition<WanderBehaviourModuleDefinition>(out var wanderModuleDefinition))
            {
                return new WanderBehaviourModule(wanderModuleDefinition.MinIdleTime, wanderModuleDefinition.MaxIdleTime, wanderModuleDefinition.WanderRadius);
            }
            return null;
        }
    }
}
