using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Querying;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class LocomotionSystem : EntitySystemBase, IFixedTickSystem, ITickSystem
    {
        private const float MoveEpsilonSqr = 0.0001f;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled,
                new[] { typeof(SpatialModule), typeof(KinematicsModule), typeof(AppearanceModule), typeof(IntentModule), typeof(StatModule) }
            );

        public LocomotionSystem(EntityRepository entityRepository) : base(entityRepository)
        {
        }

        public void FixedTick(float delta)
        {
            IterateMatchingEntities(FixedTick);
        }

        private void FixedTick(Entity entity)
        {
            var spatial = entity.GetModule<SpatialModule>();
            var intent = entity.GetModule<IntentModule>();
            var stat = entity.GetModule<StatModule>();
            var kinematics = entity.GetModule<KinematicsModule>();
            var appearance = entity.GetModule<AppearanceModule>();

            if (!intent.TryConsumeMove(out var move)) return;

            float moveSpeed = stat.Stats.Get(StatId.MoveSpeed);

            var direction = move.NormalizedDirection;
            var multiplier = move.SpeedMultiplier;
            var velocity = moveSpeed * multiplier * direction;
            kinematics.SetVelocity(velocity);

            var isMoving = velocity.sqrMagnitude > MoveEpsilonSqr;
            appearance.RequestSetAnimatorValue(EntityAnimatorParameterName.IsWalking, isMoving);
            if (isMoving)
            {
                spatial.SetFacing(velocity.normalized);
                var newTurnDirection = velocity.x >= 0f ? TurnDirection.Right : TurnDirection.Left;
                appearance.RequestSetTurnDirection(newTurnDirection);
            }
        }

        public void Tick(float delta)
        {
            IterateMatchingEntities(Tick);
        }

        private void Tick(Entity entity)
        {
            var spatial = entity.GetModule<SpatialModule>();
            var intent = entity.GetModule<IntentModule>();
            var appearance = entity.GetModule<AppearanceModule>();

            if (intent.TryConsumeAim(out var aimIntent) && aimIntent.HasPoint)
            {
                var facingDirection = aimIntent.WorldPoint - spatial.Position;
                spatial.SetFacing(facingDirection);

                var newTurnDirection = spatial.Facing.x >= 0f ? TurnDirection.Right : TurnDirection.Left;
                appearance.RequestSetTurnDirection(newTurnDirection);
            }
        }
    }
}
