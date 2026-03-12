using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control.Intents
{
    public readonly struct AimIntent : IIntent
    {
        public readonly Vector2 WorldPoint;
        public readonly bool HasPoint;

        IIntent IIntent.None => None;
        public static AimIntent None => default;

        public AimIntent(Vector2 worldPoint)
        {
            WorldPoint = worldPoint;
            HasPoint = true;
        }
    }
}
