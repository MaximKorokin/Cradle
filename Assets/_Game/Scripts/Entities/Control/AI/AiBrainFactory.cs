using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class AiBrainFactory
    {
        private readonly ActionEvaluator _actionEvaluator;

        public AiBrainFactory(ActionEvaluator actionEvaluator)
        {
            _actionEvaluator = actionEvaluator;
        }

        public AiBrain Create(AiBehaviour behaviour)
        {
            var behaviours = new List<IAiBehaviour>();
            if (behaviour.HasFlag(AiBehaviour.Wander))
            {
                behaviours.Add(new WanderBehaviour());
            }
            if (behaviour.HasFlag(AiBehaviour.Action))
            {
                behaviours.Add(new ActionBehaviour(_actionEvaluator));
            }

            return new(behaviours);
        }
    }
}
