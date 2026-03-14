using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    public sealed class AiBrain
    {
        private readonly List<IAiBehaviour> _behaviours;

        public AiBrain(IEnumerable<IAiBehaviour> behaviours)
        {
            _behaviours = behaviours.ToList();
        }

        public void Tick(Entity entity, float delta)
        {
            IAiBehaviour bestBehaviour = null;
            ActionContext bestContext = default;
            float bestScore = float.MinValue;

            var behaviours = _behaviours;
            var count = behaviours.Count;

            for (int i = 0; i < count; i++)
            {
                var behaviour = behaviours[i];
                var evaluation = behaviour.Evaluate(entity);

                if (evaluation.Score > bestScore)
                {
                    bestScore = evaluation.Score;
                    bestBehaviour = behaviour;
                    bestContext = evaluation.Context;
                }
            }

            bestBehaviour?.Tick(entity, bestContext, delta);
        }
    }

    [Flags]
    public enum AiBehaviour
    {
        Wander = 1,
        Chase = 2,
        Action = 4,
    }
}
