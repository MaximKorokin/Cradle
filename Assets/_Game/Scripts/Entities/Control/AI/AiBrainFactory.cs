using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class AiBrainFactory
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly ActionEvaluator _actionEvaluator;
        private readonly IEntitySensor _entitySensor;

        public AiBrainFactory(
            IGlobalEventBus globalEventBus,
            ActionEvaluator actionEvaluator,
            IEntitySensor entitySensor)
        {
            _globalEventBus = globalEventBus;
            _actionEvaluator = actionEvaluator;
            _entitySensor = entitySensor;
        }

        public AiBrain Create(AiBehaviour behaviour)
        {
            var behaviours = new List<IAiBehaviour>();
            if (behaviour.HasFlag(AiBehaviour.Wander))
            {
                behaviours.Add(new WanderBehaviour());
            }
            if (behaviour.HasFlag(AiBehaviour.StandStill))
            {
                behaviours.Add(new StandStillBehaviour());
            }
            if (behaviour.HasFlag(AiBehaviour.Action))
            {
                behaviours.Add(new ActionBehaviour(_actionEvaluator));
            }
            if (behaviour.HasFlag(AiBehaviour.Shop))
            {
                behaviours.Add(new ShopBehaviour(_globalEventBus, _entitySensor));
            }

            return new(behaviours);
        }
    }
}
