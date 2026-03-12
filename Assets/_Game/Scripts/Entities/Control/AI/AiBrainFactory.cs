using Assets._Game.Scripts.Infrastructure;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class AiBrainFactory
    {
        private readonly IEntitySensor _entitySensor;

        public AiBrainFactory(IEntitySensor entitySensor)
        {
            _entitySensor = entitySensor;
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
                behaviours.Add(new ChaseBehaviour(_entitySensor));
            }
            if (behaviour.HasFlag(AiBehaviour.Action))
            {
                behaviours.Add(new AttackBehaviour(_entitySensor));
            }

            return new(behaviours);
        }
    }
}
