namespace Assets._Game.Scripts.Entities.Units
{
    public sealed class EntityUnitsControllerFactory
    {
        private readonly EntityUnitFactory _entityUnitFactory;

        public EntityUnitsControllerFactory(EntityUnitFactory entityUnitFactory)
        {
            _entityUnitFactory = entityUnitFactory;
        }

        public EntityUnitsController Create(EntityView entityView, EntityVisualModel entityVisualModel, string variantName)
        {
            var unitsController = new EntityUnitsController(entityView.UnitsRoot, entityView.UnitsAnimator);

            foreach (var unitVisualModel in entityVisualModel.Units)
            {
                unitsController.AddUnit(_entityUnitFactory.Create(unitVisualModel, variantName));
            }

            unitsController.UpdateOrderInLayer();

            return unitsController;
        }
    }
}
