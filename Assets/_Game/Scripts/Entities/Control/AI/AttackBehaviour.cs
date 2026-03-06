using Assets._Game.Scripts.Entities.Control.Intents;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class AttackBehaviour : AiBehaviourBase
    {
        private const float AttackRange = 0.7f;

        private readonly IWorldQuery _worldQuery;

        public AttackBehaviour(IWorldQuery worldQuery)
        {
            _worldQuery = worldQuery;
        }

        public override float Evaluate()
        {
            var position = Entity.GetModule<SpatialModule>().Position;

            var enemies = _worldQuery.GetEntitiesInRange(position, AttackRange).ToList();
            enemies.Remove(Entity);

            return enemies.Any() ? 1.0f : 0f;
        }

        public override void Execute(float delta)
        {
            var intent = Entity.GetModule<IntentModule>();

            var target = _worldQuery
                .GetEntitiesInRange(Entity.GetModule<SpatialModule>().Position, AttackRange)
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
