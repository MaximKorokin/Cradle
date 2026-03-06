using Assets._Game.Scripts.Entities.Control.Intents;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class ChaseBehaviour : AiBehaviourBase
    {
        private const float MaxSpotLeaveDistance = 3;
        private const float ChaseStartDistance = 3;

        private readonly IWorldQuery _worldQuery;

        public ChaseBehaviour(IWorldQuery worldQuery)
        {
            _worldQuery = worldQuery;
        }

        public override float Evaluate()
        {
            var position = Entity.GetModule<SpatialModule>().Position;

            var enemies = _worldQuery.GetEntitiesInRange(position, ChaseStartDistance).ToList();
            enemies.Remove(Entity);

            return enemies.Any() ? 0.8f : 0f;
        }

        public override void Execute(float delta)
        {
            var spatial = Entity.GetModule<SpatialModule>();
            var intent = Entity.GetModule<IntentModule>();

            var target = _worldQuery
                .GetEntitiesInRange(spatial.Position, ChaseStartDistance)
                .FirstOrDefault();

            if (target == null)
                return;

            var direction = target.GetModule<SpatialModule>().Position - spatial.Position;

            intent.SetMove(new MoveIntent(direction));
        }
    }
}
