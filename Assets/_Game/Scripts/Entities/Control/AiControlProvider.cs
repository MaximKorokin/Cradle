using Assets._Game.Scripts.Entities.Modules;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class AiControlProvider : IControlProvider
    {
        public ControlPriority Priority => ControlPriority.BaseAI;
        public ControlMask Mask => ControlMask.All;
        public bool IsActive => true;

        public void Tick(Entity entity, float delta)
        {
            // todo: implement ai control
            if (entity.TryGetModule(out IntentModule intent))
            {
                intent.SetMove(new(Vector2.up));
            }
        }
    }

    [Serializable]
    public sealed class AiControlProviderData : ControlProviderData
    {
        public override IControlProvider CreateInstance() => new AiControlProvider();
    }
}
