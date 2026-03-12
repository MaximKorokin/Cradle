using Assets._Game.Scripts.Entities.Control.Intents;
using Assets._Game.Scripts.Entities.Faction;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure;
using Assets._Game.Scripts.Shared.Extensions;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class WorldActionBehaviour : AiBehaviourBase
    {
        private const float ActionRange = 0.7f;

        private readonly EntityQuery _entityQuery = new(RestrictionState.Disabled | RestrictionState.Dead);
        private readonly IEntitySensor _sensor;

        public WorldActionBehaviour(IEntitySensor sensor)
        {
            _sensor = sensor;
        }

        public override float Evaluate()
        {
            return _sensor.HasAnyInRange(Entity, ActionRange, FactionRelation.Enemy, _entityQuery)
                ? 1.0f
                : 0f;
        }

        public override void Execute(float delta)
        {
            if (!_sensor.TryGetFirstInRange(Entity, ActionRange, FactionRelation.Enemy, _entityQuery, out var target))
                return;

            if (!Entity.TryGetModule<ActionModule>(out var actionModule))
                return;

            var action = actionModule.Actions.FirstOrDefault();
            if (action == null)
                return;

            var targetPosition = target.GetModule<SpatialModule>().Position;
            var intent = Entity.GetModule<IntentModule>();

            intent.SetAct(new ActIntent(action, target, targetPosition));
        }
    }
}
