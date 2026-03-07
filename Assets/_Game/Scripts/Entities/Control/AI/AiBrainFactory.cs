using Assets._Game.Scripts.Entities.Faction;
using Assets._Game.Scripts.Infrastructure;
using System.Collections.Generic;
using VContainer;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class AiBrainFactory
    {
        private readonly IWorldQuery _worldQuery;
        private readonly FactionRelationResolver _relationResolver;

        public AiBrainFactory(IWorldQuery worldQuery, FactionRelationResolver relationResolver)
        {
            _worldQuery = worldQuery;
            _relationResolver = relationResolver;
        }

        public AiBrain Create(AiBehaviour behaviour)
        {
            var behaviours = new List<IAiBehaviour>();
            if (behaviour.HasFlag(AiBehaviour.Wander))
            {
                behaviours.Add(new WanderBehaviour());
            }
            if (behaviour.HasFlag(AiBehaviour.Chase))
            {
                behaviours.Add(new ChaseBehaviour(_worldQuery, _relationResolver));
            }
            if (behaviour.HasFlag(AiBehaviour.Attack))
            {
                behaviours.Add(new AttackBehaviour(_worldQuery, _relationResolver));
            }

            return new(behaviours);
        }
    }
}
