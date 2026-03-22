using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public sealed class UnitsControllerFactory
    {
        private readonly UnitViewProvider _unitViewProvider;

        public UnitsControllerFactory(UnitViewProvider unitViewProvider)
        {
            _unitViewProvider = unitViewProvider;
        }

        public UnitsController Create(Transform unitsRoot, EntityVisualModel entityVisualModel, string variantName)
        {
            var unitsController = new UnitsController(unitsRoot, _unitViewProvider);

            foreach (var unitVisualModel in entityVisualModel.Units)
            {
                unitsController.AddUnit(_unitViewProvider.Create(unitVisualModel, variantName));
            }

            unitsController.UpdateOrderInLayer();

            return unitsController;
        }
    }
}
