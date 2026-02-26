using Assets._Game.Scripts.Infrastructure;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules.Positioning
{
    public sealed class EntityPositioningFactory
    {
        private readonly DispatcherService _dispatcherService;

        public EntityPositioningFactory(DispatcherService dispatcherService)
        {
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
                controlModule.AddProvider(controlModuleDefinition.ControlProvider.CreateInstance());
            }
            yield return controlModule;
        }
    }
}
