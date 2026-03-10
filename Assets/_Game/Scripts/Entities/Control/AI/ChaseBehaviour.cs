using Assets._Game.Scripts.Entities.Control.Intents;
using Assets._Game.Scripts.Entities.Faction;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure;
using Assets._Game.Scripts.Shared.Extensions;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class ChaseBehaviour : AiBehaviourBase
    {
        private const float MaxSpotLeaveDistance = 3f;
        private const float ChaseStartDistance = 3f;

        private readonly EntityQuery _entityQuery = new(RestrictionState.Disabled | RestrictionState.Dead);
        private readonly IEntitySensor _sensor;

        public ChaseBehaviour(IEntitySensor sensor)
        {
            _sensor = sensor;
        }

        public override float Evaluate()
        {
            return _sensor.HasAnyInRange(Entity, ChaseStartDistance, FactionRelation.Enemy, _entityQuery) ? 0.8f : 0f;
        }

        public override void Execute(float delta)
        {
            if (!_sensor.TryGetFirstInRange(Entity, ChaseStartDistance, FactionRelation.Enemy, _entityQuery, out var target))
                return;

            var position = Entity.GetModule<SpatialModule>().Position;
            var targetPosition = target.GetModule<SpatialModule>().Position;
            var direction = targetPosition - position;

            var intent = Entity.GetModule<IntentModule>();
            intent.SetMove(new MoveIntent(direction));
        }
    }
}
