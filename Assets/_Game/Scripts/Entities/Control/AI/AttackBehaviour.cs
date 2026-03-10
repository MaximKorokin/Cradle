using Assets._Game.Scripts.Entities.Control.Intents;
using Assets._Game.Scripts.Entities.Faction;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure;
using Assets._Game.Scripts.Shared.Extensions;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class AttackBehaviour : AiBehaviourBase
    {
        private const float AttackRange = 0.7f;

        private readonly EntityQuery _entityQuery = new(RestrictionState.Disabled | RestrictionState.Dead);
        private readonly IEntitySensor _sensor;

        public AttackBehaviour(IEntitySensor sensor)
        {
            _sensor = sensor;
        }

        public override float Evaluate()
        {
            return _sensor.HasAnyInRange(Entity, AttackRange, FactionRelation.Enemy, _entityQuery)
                ? 1.0f
                : 0f;
        }

        public override void Execute(float delta)
        {
            if (!_sensor.TryGetFirstInRange(Entity, AttackRange, FactionRelation.Enemy, _entityQuery, out var target))
                return;

            if (!Entity.TryGetModule<AbilityModule>(out var abilityModule))
                return;

            var ability = abilityModule.Abilities.FirstOrDefault();
            if (ability == null)
                return;

            var targetPosition = target.GetModule<SpatialModule>().Position;
            var intent = Entity.GetModule<IntentModule>();

            intent.SetUseAbility(new UseAbilityIntent(ability, target, targetPosition));
        }
    }
}
