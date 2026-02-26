using Assets._Game.Scripts.Entities.Modules;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class FearControlProvider : TimedControlProvider
    {
        public override ControlPriority Priority => ControlPriority.OverrideLow;
        public override ControlMask Mask => ControlMask.All;

        private readonly float _speedMultiplier;
        private Vector2 _direction;

        public FearControlProvider(float duration, float speedMultiplier) : base(duration)
        {
            _speedMultiplier = speedMultiplier;
        }

        public void InitializeDirection(Vector2 direction)
        {
            _direction = direction.normalized;
        }

        protected override void OnTick(Entity entity, float delta)
        {
            if (entity.TryGetModule(out IntentModule intent))
                intent.SetMove(_direction, _speedMultiplier);
        }
    }

    [Serializable]
    public sealed class FearControlProviderData : ControlProviderData
    {
        public float Duration = 2f;
        public float SpeedMultiplier = 1f;

        public override IControlProvider CreateInstance() => new FearControlProvider(Duration, SpeedMultiplier);
    }
}
