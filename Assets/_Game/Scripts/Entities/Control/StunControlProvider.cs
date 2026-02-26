using Assets._Game.Scripts.Entities.Modules;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class StunControlProvider : TimedControlProvider
    {
        public override ControlPriority Priority => ControlPriority.OverrideHigh;
        public override ControlMask Mask => ControlMask.All;

        public StunControlProvider(float duration) : base(duration)
        {
        }

        protected override void OnTick(Entity entity, float delta)
        {
            if (entity.TryGetModule(out IntentModule intent))
            {
                intent.SetMove(new(Vector2.zero, 0));
            }
        }
    }

    [Serializable]
    public sealed class StunControlProviderData : ControlProviderData
    {
        public float Duration = 2f;
        public override IControlProvider CreateInstance() => new StunControlProvider(Duration);
    }
}
