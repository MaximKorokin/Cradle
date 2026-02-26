using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control.Intents
{
    public readonly struct AimIntent
    {
        public readonly Vector2 WorldPoint;
        public readonly bool HasPoint;

        public static AimIntent None { get; } = default;

        public AimIntent(Vector2 worldPoint)
        {
            WorldPoint = worldPoint;
            HasPoint = true;
        }
    }
}
