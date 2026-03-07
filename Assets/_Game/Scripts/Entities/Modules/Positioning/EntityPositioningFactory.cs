using System.Collections.Generic;
using VContainer;

namespace Assets._Game.Scripts.Entities.Modules.Positioning
{
    public sealed class EntityPositioningFactory
    {
        private readonly IObjectResolver _resolver;

        public EntityPositioningFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public IEnumerable<EntityModuleBase> Create(EntityDefinition entityDefinition)
        {
            var spatialModule = new SpatialModule();
            yield return spatialModule;
            var kinematicsModule = new KinematicsModule();
            yield return kinematicsModule;

            var intentModule = new IntentModule();
            yield return intentModule;

            var controlModule = new ControlModule();
            if (entityDefinition.TryGetModuleDefinition<ControlModuleDefinition>(out var controlModuleDefinition))
            {
                controlModule.AddProvider(controlModuleDefinition.ControlProvider.CreateInstance(_resolver));
            }
            yield return controlModule;
        }
    }
}
