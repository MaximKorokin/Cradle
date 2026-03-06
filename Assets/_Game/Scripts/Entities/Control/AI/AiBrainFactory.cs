using Assets._Game.Scripts.Infrastructure;
using System.Collections.Generic;
using VContainer;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class AiBrainFactory
    {
        private readonly IWorldQuery _worldQuery;

        public AiBrainFactory(IObjectResolver resolver)
        {
            _worldQuery = resolver.Resolve<IWorldQuery>();
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
                behaviours.Add(new ChaseBehaviour(_worldQuery));
            }
            if (behaviour.HasFlag(AiBehaviour.Attack))
            {
                behaviours.Add(new AttackBehaviour(_worldQuery));
            }

            return new(behaviours);
        }
    }
}
