using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Shared.Extensions;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class LocomotionSystem : EntitySystemBase, IFixedTickSystem
    {
        private const float MoveEpsilonSqr = 0.0001f;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled,
                new[] { typeof(SpatialModule), typeof(KinematicsModule), typeof(AppearanceModule) }
            );

        public LocomotionSystem(EntityRepository entityRepository) : base(entityRepository)
        {
        }

        public void FixedTick(float delta)
        {
            IterateMatchingEntities(FixedTick);
        }

        public void FixedTick(Entity entity)
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
    }
}
