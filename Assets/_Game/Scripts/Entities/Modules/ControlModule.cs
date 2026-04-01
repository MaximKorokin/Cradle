using Assets._Game.Scripts.Entities.Control;
using System.Collections.Generic;
using VContainer;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class ControlModule : EntityModuleBase
    {
        private readonly List<IControlProvider> _providers = new();
        public IReadOnlyList<IControlProvider> Providers => _providers;

        public const int ControlMaskCount = 3;
        public readonly IControlProvider[] RawControlProviders = new IControlProvider[ControlMaskCount];
        public readonly IControlProvider[] UniqueControlProviders = new IControlProvider[ControlMaskCount];

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

        public void ResetProviders()
        {
            for (int i = 0; i < Providers.Count; i++)
            {
                Providers[i].Reset();
            }
        }
    }

    public sealed class ControlModuleFactory : IEntityModuleFactory
    {
        private readonly IObjectResolver _resolver;

        public ControlModuleFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModuleDefinition<ControlModuleDefinition>(out var controlModuleDefinition))
            {
                return null;
            }

            var controlModule = new ControlModule();
            if (controlModuleDefinition.ControlProvider != null)
            {
                controlModule.AddProvider(controlModuleDefinition.ControlProvider.CreateInstance(_resolver));
            }
            return controlModule;
        }
    }
}
