using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control.Intents
{
    public readonly struct AimIntent : IIntent
    {
        public readonly Vector2 WorldPoint;
        public readonly bool HasPoint;

        private readonly bool _hasIntent;
        readonly bool IIntent.HasIntent => _hasIntent;

        public AimIntent(Vector2 worldPoint)
        {
            WorldPoint = worldPoint;
            HasPoint = true;

            _hasIntent = true;
        }
    }
}
