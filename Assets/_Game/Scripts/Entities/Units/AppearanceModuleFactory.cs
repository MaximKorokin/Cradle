using Assets._Game.Scripts.Entities.Modules;

namespace Assets._Game.Scripts.Entities.Units
{
    public sealed class AppearanceModuleFactory
    {
        private readonly EntityUnitsControllerFactory _unitsControllerFactory;
        private readonly EntityUnitFactory _entityUnitFactory;

        public AppearanceModuleFactory(EntityUnitsControllerFactory unitsControllerFactory, EntityUnitFactory entityUnitFactory)
        {
            _unitsControllerFactory = unitsControllerFactory;
            _entityUnitFactory = entityUnitFactory;
        }

        public AppearanceModule Create(EntityView entityView, EntityDefinition entityDefinition)
        {
            var unitsController = _unitsControllerFactory.Create(entityView, entityDefinition.VisualModel, entityDefinition.VariantName);
            var appearanceModel = new AppearanceModule(entityDefinition.VisualModel, unitsController, _entityUnitFactory);
            return appearanceModel;
        }
    }
}
