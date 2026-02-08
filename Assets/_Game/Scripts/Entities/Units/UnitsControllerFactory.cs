namespace Assets._Game.Scripts.Entities.Units
{
    public sealed class UnitsControllerFactory
    {
        private readonly UnitFactory _entityUnitFactory;

        public UnitsControllerFactory(UnitFactory entityUnitFactory)
        {
            _entityUnitFactory = entityUnitFactory;
        }

        public UnitsController Create(EntityView entityView, EntityVisualModel entityVisualModel, string variantName)
        {
            var unitsController = new UnitsController(entityView.UnitsRoot, entityView.UnitsAnimator, entityVisualModel.Animator);

            foreach (var unitVisualModel in entityVisualModel.Units)
            {
                unitsController.AddUnit(_entityUnitFactory.Create(unitVisualModel, variantName));
            }

            unitsController.UpdateOrderInLayer();

            return unitsController;
        }
    }
}
