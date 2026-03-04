using UnityEngine;

namespace Assets._Game.Scripts.Entities.Interactions
{
    public readonly struct InteractionContext
    {
        public readonly Entity Source;
        public readonly Entity Target;
        public readonly Vector2? Point;

        public InteractionContext(Entity source, Entity target, Vector2? point)
        {
            Source = source;
            Target = target;
            Point = point;
        }
    }
}
