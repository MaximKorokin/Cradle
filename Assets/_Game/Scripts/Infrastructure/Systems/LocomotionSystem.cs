using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Shared.Extensions;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class LocomotionSystem : EntitySystemBase
    {
        private const float MoveEpsilonSqr = 0.0001f;

        protected override EntityQuery EntityQuery => new(RestrictionState.Disabled);

        public LocomotionSystem(EntityRepository entityRepository, DispatcherService dispatcher) : base(entityRepository, dispatcher)
        {
            FixedTickAction += FixedTick;
        }

        public void FixedTick(Entity entity, float delta)
        {
            var spatial = entity.GetModule<SpatialModule>();
            var kinematics = entity.GetModule<KinematicsModule>();
            var appearance = entity.GetModule<AppearanceModule>();

            var velocity = kinematics.Velocity;

            var newTurnDirection = spatial.Facing.x >= 0f ? TurnDirection.Right : TurnDirection.Left;
            appearance.RequestSetTurnDirection(newTurnDirection);

            var isMoving = velocity.sqrMagnitude > MoveEpsilonSqr;
            appearance.RequestSetAnimatorValue(EntityAnimatorParameterName.IsWalking, isMoving);
        }

        protected override bool Filter(Entity entity)
        {
            return entity.HasModule<SpatialModule>() && entity.HasModule<KinematicsModule>() && entity.HasModule<AppearanceModule>();
        }
    }
}
