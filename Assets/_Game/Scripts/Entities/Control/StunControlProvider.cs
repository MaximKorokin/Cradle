using Assets._Game.Scripts.Entities.Control.Intents;
using Assets._Game.Scripts.Entities.Modules;
using System;
using VContainer;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class StunControlProvider : TimedControlProvider
    {
        public override ControlPriority Priority => ControlPriority.OverrideHigh;
        public override ControlMask Mask => ControlMask.All;

        public StunControlProvider(float duration) : base(duration)
        {
        }

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);

            Entity.GetModule<RestrictionStateModule>().Add(RestrictionState.Stunned);
            if (Entity.TryGetModule(out IntentModule intent))
            {
                intent.SetMove(MoveIntent.None);
                intent.SetAct(ActionIntent.None);
            }
        }

        protected override void OnTick(float delta)
        {
        }

        protected override void OnComplete()
        {
            Entity.GetModule<RestrictionStateModule>().Remove(RestrictionState.Stunned);
        }
    }

    [Serializable]
    public sealed class StunControlProviderData : TimedControlProviderData
    {
        public override IControlProvider CreateInstance(IObjectResolver resolver) => new StunControlProvider(Duration);
    }
}
