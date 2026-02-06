using Assets._Game.Scripts.Entities.Units;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class EntityAppearanceModuleFactory
    {
        private readonly EntityUnitsControllerFactory _unitsControllerFactory;
        private readonly EntityUnitFactory _entityUnitFactory;

        public EntityAppearanceModuleFactory(EntityUnitsControllerFactory unitsControllerFactory, EntityUnitFactory entityUnitFactory)
        {
            _unitsControllerFactory = unitsControllerFactory;
            _entityUnitFactory = entityUnitFactory;
        }

        public EntityAppearanceModule Create(EntityView entityView, EntityDefinition entityDefinition)
        {
            var unitsController = _unitsControllerFactory.Create(entityView, entityDefinition.VisualModel, entityDefinition.VariantName);
            var appearanceModel = new EntityAppearanceModule(entityDefinition.VisualModel, unitsController, _entityUnitFactory);
            return appearanceModel;
        }
    }
}
