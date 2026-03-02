using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Units;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class LocomotionSystem : SystemBase
    {
        private const float MoveEpsilonSqr = 0.0001f;

        private readonly EntityRepository _entityRepository;
        private readonly DispatcherService _dispatcher;

        public LocomotionSystem(EntityRepository entityRepository, DispatcherService dispatcher)
        {
            _entityRepository = entityRepository;
            _dispatcher = dispatcher;

            _dispatcher.OnFixedTick += OnFixedTick;
        }

        private void OnFixedTick(float delta)
        {
            foreach (var entity in _entityRepository.All)
            {
                Tick(entity, delta);
            }
        }

        public void Tick(Entity entity, float delta)
        {
            if (!entity.TryGetModule(out SpatialModule spatial)) return;
            if (!entity.TryGetModule(out KinematicsModule kinematics)) return;
            if (!entity.TryGetModule(out AppearanceModule appearance)) return;

            var velocity = kinematics.Velocity;

            var newTurnDirection = spatial.Facing.x >= 0f ? TurnDirection.Right : TurnDirection.Left;
            appearance.RequestSetTurnDirection(newTurnDirection);

            var isMoving = velocity.sqrMagnitude > MoveEpsilonSqr;
            appearance.RequestSetAnimatorValue(EntityAnimatorParameterName.IsWalking, isMoving);
        }

        public override void Dispose()
        {
            base.Dispose();
            _dispatcher.OnFixedTick -= OnFixedTick;
        }
    }
}
