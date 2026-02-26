using Assets._Game.Scripts.Entities.Interactions;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control.Intents
{
    public readonly struct InteractIntent
    {
        public readonly InteractionDefinition InteractionDefinition;
        public readonly UseKind Kind;
        // can be null if the ability doesn't require a target
        public readonly Entity Target;
        // for ground-targeted abilities, the point where the ability should be used. If HasPoint is false, this value should be ignored.
        public readonly Vector2 Point;
        public readonly bool HasPoint;

        public InteractIntent(InteractionDefinition interactionDefinition, UseKind kind, Entity target, Vector2 point, bool hasPoint)
        {
            InteractionDefinition = interactionDefinition;
            Kind = kind;
            Target = target;
            Point = point;
            HasPoint = hasPoint;
        }

        public static InteractIntent None() => default;
    }

    public enum UseKind
    {
        None,
        Press,
        Hold,
        Release
    }
}
