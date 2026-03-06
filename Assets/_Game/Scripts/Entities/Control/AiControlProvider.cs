using Assets._Game.Scripts.Entities.Control.AI;
using System;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class AiControlProvider : IControlProvider
    {
        public ControlPriority Priority => ControlPriority.BaseAI;
        public ControlMask Mask => ControlMask.All;
        public bool IsActive => true;

        private Entity _entity;

        private readonly AiBrain _brain;

        public AiControlProvider(AiBrain brain)
        {
            _brain = brain;
        }

        public void Initialize(Entity entity)
        {
            _entity = entity;
            _brain.Initialize(entity);
        }

        public void Tick(float delta)
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
