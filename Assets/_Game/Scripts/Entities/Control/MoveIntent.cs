using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control
{
    public readonly struct MoveIntent
    {
        public readonly Vector2 NormalizedDirection;
        public readonly float SpeedMultiplier;

        public MoveIntent(Vector2 direction, float speedMultiplier = 1f)
        {
            NormalizedDirection = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.zero;
            SpeedMultiplier = speedMultiplier;
        }

        public static MoveIntent Stopping() => new MoveIntent(default) { };
    }
}
