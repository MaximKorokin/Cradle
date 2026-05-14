using Assets._Game.Scripts.Shared.Extensions;
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

        public UnitsController Create(Transform unitsRoot, EntityVisualModel entityVisualModel)
        {
            var unitsController = new UnitsController(unitsRoot, _unitViewProvider, entityVisualModel.SwapOrderInLayerForDirection);
            var colorVariant = entityVisualModel.ColorVariants.GetRandomElement(Color.white);

            foreach (var unitVisualModel in entityVisualModel.Units)
            {
                unitsController.AddUnit(_unitViewProvider.Create(unitVisualModel, colorVariant));
            }

            unitsController.UpdateOrderInLayer();

            return unitsController;
        }
    }
}
