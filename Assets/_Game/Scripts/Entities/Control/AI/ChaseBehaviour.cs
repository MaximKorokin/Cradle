using Assets._Game.Scripts.Entities.Control.Intents;
using Assets._Game.Scripts.Entities.Faction;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure;
using Assets._Game.Scripts.Shared.Extensions;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class ChaseBehaviour : AiBehaviourBase
    {
        private const float MaxSpotLeaveDistance = 3;
        private const float ChaseStartDistance = 3;

        private readonly EntityQuery _entityQuery = new(RestrictionState.Disabled | RestrictionState.Dead);
        private readonly IWorldQuery _worldQuery;
        private readonly FactionRelationResolver _relationResolver;

        public ChaseBehaviour(IWorldQuery worldQuery, FactionRelationResolver relationResolver)
        {
            _worldQuery = worldQuery;
            _relationResolver = relationResolver;
        }

        public override float Evaluate()
        {
            var position = Entity.GetModule<SpatialModule>().Position;
            var enemies = _worldQuery
                .GetEntitiesInRange(position, ChaseStartDistance, Entity, FactionRelation.Enemy, _relationResolver)
                .Query(_entityQuery);

            return enemies.Any() ? 0.8f : 0f;
        }

        public override void Execute(float delta)
        {
            var position = Entity.GetModule<SpatialModule>().Position;
            var intent = Entity.GetModule<IntentModule>();

            var target = _worldQuery
                .GetEntitiesInRange(position, ChaseStartDistance, Entity, FactionRelation.Enemy, _relationResolver)
                .Query(_entityQuery)
                .FirstOrDefault();

            if (target == null)
                return;

            var direction = target.GetModule<SpatialModule>().Position - position;

            intent.SetMove(new MoveIntent(direction));
        }
    }
}
