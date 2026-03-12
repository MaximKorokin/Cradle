using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control.Intents
{
    public readonly struct MoveIntent : IIntent
    {
        public readonly Vector2 NormalizedDirection;
        public readonly float SpeedMultiplier;

        IIntent IIntent.None => None;
        public static MoveIntent None => default;

        public MoveIntent(Vector2 direction, float speedMultiplier = 1f)
        {
            NormalizedDirection = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.zero;
            SpeedMultiplier = speedMultiplier;
        }
    }
}
