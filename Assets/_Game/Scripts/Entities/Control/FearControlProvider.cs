using Assets._Game.Scripts.Entities.Modules;
using System;
using UnityEngine;
using VContainer;

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

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);

            _direction = UnityEngine.Random.insideUnitCircle;
            Entity.GetModule<RestrictionStateModule>().Add(RestrictionState.Feared);
        }

        protected override void OnTick(float delta)
        {
            if (Entity.TryGetModule(out IntentModule intent))
                intent.SetMove(new(_direction, _speedMultiplier));
        }

        protected override void OnComplete()
        {
            Entity.GetModule<RestrictionStateModule>().Remove(RestrictionState.Feared);
        }
    }

    [Serializable]
    public sealed class FearControlProviderData : TimedControlProviderData
    {
        public float SpeedMultiplier = 1f;

        public override IControlProvider CreateInstance(IObjectResolver resolver) => new FearControlProvider(Duration, SpeedMultiplier);
    }
}
