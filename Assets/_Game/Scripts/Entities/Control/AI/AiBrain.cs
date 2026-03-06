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

        public void Initialize(Entity entity)
        {
            for (int i = 0; i < _behaviours.Count; i++)
            {
                _behaviours[i].Initialize(entity);
            }
        }

        public void Tick(float delta)
        {
            IAiBehaviour bestBehaviour = null;
            float bestScore = float.MinValue;

            var behaviours = _behaviours;
            var count = behaviours.Count;

            for (int i = 0; i < count; i++)
            {
                var behaviour = behaviours[i];
                var score = behaviour.Evaluate();

                if (score > bestScore)
                {
                    bestScore = score;
                    bestBehaviour = behaviour;
                }
            }

            bestBehaviour?.Execute(delta);
        }
    }

    [Flags]
    public enum AiBehaviour
    {
        Wander = 1,
        Chase = 2,
        Attack = 4,
    }
}
