using Assets._Game.Scripts.Entities.Control;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class ControlModule : EntityModuleBase
    {
        private readonly List<IControlProvider> _providers = new();
        public IReadOnlyList<IControlProvider> Providers => _providers;

        public readonly IControlProvider[] RawControlProviders = new IControlProvider[100];
        public readonly IControlProvider[] UniqueControlProviders = new IControlProvider[100];

        protected override void OnAttach()
        {
            foreach (var provider in _providers)
            {
                provider.Initialize(Entity);
            }
        }

        public void AddProvider(IControlProvider p)
        {
            if (Entity != null)
            {
                p.Initialize(Entity);
            }

            _providers.Add(p);
        }

        public void RemoveProvider(IControlProvider p)
        {
            _providers.Remove(p);
        }
    }

    public readonly struct OverrideControlRequestEvent : IEntityEvent
    {
        public readonly IControlProvider ControlProvider;

        public Entity Entity { get; }

        public OverrideControlRequestEvent(Entity entity, IControlProvider controlProvider)
        {
            Entity = entity;
            ControlProvider = controlProvider;
        }
    }
}
