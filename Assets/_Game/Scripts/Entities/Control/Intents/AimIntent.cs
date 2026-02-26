using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control.Intents
{
    public readonly struct AimIntent
    {
        public readonly Vector2 WorldPoint;
        public readonly bool HasPoint;

        public AimIntent(Vector2 worldPoint)
        {
            WorldPoint = worldPoint;
            HasPoint = true;
        }

        public static AimIntent None() => default;
    }
}
