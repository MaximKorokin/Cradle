using Assets._Game.Scripts.Infrastructure;
using System.Collections.Generic;
using VContainer;

namespace Assets._Game.Scripts.Entities.Modules.Positioning
{
    public sealed class EntityPositioningFactory
    {
        private readonly IObjectResolver _resolver;
        private readonly DispatcherService _dispatcherService;

        public EntityPositioningFactory(IObjectResolver resolver, DispatcherService dispatcherService)
        {
            _resolver = resolver;
            _dispatcherService = dispatcherService;
        }

        public IEnumerable<EntityModuleBase> Create(EntityDefinition entityDefinition)
        {
            var spatialModule = new SpatialModule();
            yield return spatialModule;
            var kinematicsModule = new KinematicsModule();
            yield return kinematicsModule;

            var intentModule = new IntentModule();
            yield return intentModule;

            var controlModule = new ControlModule(_dispatcherService);
            if (entityDefinition.TryGetModuleDefinition<ControlModuleDefinition>(out var controlModuleDefinition))
            {
                controlModule.AddProvider(controlModuleDefinition.ControlProvider.CreateInstance(_resolver));
            }
            yield return controlModule;
        }
    }
}
