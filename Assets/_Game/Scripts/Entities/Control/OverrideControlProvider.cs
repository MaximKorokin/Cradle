using Assets._Game.Scripts.Entities.Modules;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class OverrideControlProvider : IControlProvider
    {
        public ControlPriority Priority => ControlPriority.OverrideLow;
        public bool IsActive => _timeLeft > 0f;

        private float _timeLeft;
        private Vector2 _direction;
        private float _speedMultiplier;

        public void Start(Vector2 direction, float duration, float speedMultiplier = 1f)
        {
            _direction = direction.sqrMagnitude > 0 ? direction.normalized : Vector2.up;
            _timeLeft = duration;
            _speedMultiplier = speedMultiplier;
        }

        public void Tick(Entity entity, float delta)
        {
            _timeLeft -= delta;
            if (entity.TryGetModule(out IntentModule intentModule))
            {
                intentModule.SetMove(_direction, _speedMultiplier);
            }
        }
    }
}
