using Assets._Game.Scripts.Entities.Control.AI;
using System;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class AiControlProvider : ControlProviderBase
    {
        public override ControlPriority Priority => ControlPriority.BaseAI;
        public override ControlMask Mask => _isEnabled ? ControlMask.All : ControlMask.None;
        public override bool IsPersisted => true;

        private bool _isEnabled = true;
        private readonly AiBrain _brain;

        public AiControlProvider(AiBrain brain)
        {
            _brain = brain;
        }

        public void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;
            if (!enabled)
                _brain.Reset();
        }

        public override void Tick(float delta)
        {
            if (_isEnabled)
                _brain.Tick(Entity, delta);
        }

        public override void Reset()
        {
            _brain.Reset();
        }
    }

    [Serializable]
    public sealed class AiControlProviderData : ControlProviderData
    {
        [field: SerializeField]
        public AiBehaviour AiBehaviours { get; private set; }

        public override IControlProvider CreateInstance(IObjectResolver resolver) => new AiControlProvider(resolver.Resolve<AiBrainFactory>().Create(AiBehaviours));
    }
}
