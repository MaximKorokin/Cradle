using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public sealed class UnitsControllerFactory
    {
        private readonly UnitFactory _unitFactory;

        public UnitsControllerFactory(UnitFactory entityUnitFactory)
        {
            _unitFactory = entityUnitFactory;
        }

        public UnitsController Create(Transform unitsRoot, Animator unitsAnimator, EntityVisualModel entityVisualModel, string variantName)
        {
            var unitsController = new UnitsController(unitsRoot, unitsAnimator, _unitFactory, entityVisualModel.Animator);

            foreach (var unitVisualModel in entityVisualModel.Units)
            {
                unitsController.AddUnit(_unitFactory.Create(unitVisualModel, variantName));
            }

            unitsController.UpdateOrderInLayer();

            return unitsController;
        }
    }
}
