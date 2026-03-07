using Assets._Game.Scripts.Entities.Control.Intents;
using Assets._Game.Scripts.Entities.Faction;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure;
using Assets._Game.Scripts.Shared.Extensions;
using System.Linq;
using UnityEngine.UIElements;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class AttackBehaviour : AiBehaviourBase
    {
        private const float AttackRange = 0.7f;

        private readonly EntityQuery _entityQuery = new(RestrictionState.Disabled | RestrictionState.Dead);
        private readonly IWorldQuery _worldQuery;
        private readonly FactionRelationResolver _relationResolver;

        public AttackBehaviour(IWorldQuery worldQuery, FactionRelationResolver relationResolver)
        {
            _worldQuery = worldQuery;
            _relationResolver = relationResolver;
        }

        public override float Evaluate()
        {
            var position = Entity.GetModule<SpatialModule>().Position;
            var enemies = _worldQuery
                .GetEntitiesInRange(position, AttackRange, Entity, FactionRelation.Enemy, _relationResolver)
                .Query(_entityQuery);

            return enemies.Any() ? 1.0f : 0f;
        }

        public override void Execute(float delta)
        {
            var position = Entity.GetModule<SpatialModule>().Position;
            var intent = Entity.GetModule<IntentModule>();

            var target = _worldQuery
                .GetEntitiesInRange(position, AttackRange, Entity, FactionRelation.Enemy, _relationResolver)
                .Query(_entityQuery)
                .FirstOrDefault();

            if (target == null)
                return;

            if (Entity.TryGetModule<AbilityModule>(out var abilityModule))
            {
                intent.SetUseAbility(new UseAbilityIntent(abilityModule.Abilities.First(), target, target.GetModule<SpatialModule>().Position));
            }
        }
    }
}
