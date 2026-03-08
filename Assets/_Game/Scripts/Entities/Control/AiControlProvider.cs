using Assets._Game.Scripts.Entities.Control.AI;
using System;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class AiControlProvider : ControlProviderBase
    {
        public override ControlPriority Priority => ControlPriority.BaseAI;
        public override ControlMask Mask => ControlMask.All;
        public override bool IsActive => true;

        private readonly AiBrain _brain;

        public AiControlProvider(AiBrain brain)
        {
            _brain = brain;
        }

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);

            _brain.Initialize(entity);
        }

        public override void Tick(float delta)
        {
            _brain.Tick(delta);
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
