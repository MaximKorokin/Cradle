using Assets._Game.Scripts.Entities.Units;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class AppearanceModuleFactory
    {
        private readonly UnitsControllerFactory _unitsControllerFactory;
        private readonly UnitFactory _entityUnitFactory;

        public AppearanceModuleFactory(UnitsControllerFactory unitsControllerFactory, UnitFactory entityUnitFactory)
        {
            _unitsControllerFactory = unitsControllerFactory;
            _entityUnitFactory = entityUnitFactory;
        }

        public AppearanceModule Create(EntityView entityView, EntityDefinition entityDefinition)
        {
            var unitsController = _unitsControllerFactory.Create(entityView, entityDefinition.VisualModel, entityDefinition.VariantName);
            var appearanceModel = new AppearanceModule(unitsController, _entityUnitFactory);
            return appearanceModel;
        }
    }
}
