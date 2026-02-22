using UnityEngine;

namespace Assets._Game.Scripts.Entities.Interaction
{
    public readonly struct InteractionContext
    {
        public readonly Entity Source;
        public readonly Entity Target;
        public readonly Vector2 Point;
        public readonly IGlobalEventBus GlobalBus;

        public InteractionContext(Entity source, Entity target, Vector2 point, IGlobalEventBus globalBus)
        {
            Source = source;
            Target = target;
            Point = point;
            GlobalBus = globalBus;
        }
    }
}
