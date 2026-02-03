using Assets.CoreScripts;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public sealed class UnitsControllerFactory
    {
        private readonly EntityUnitVariantsCatalog _entityUnitsManager;
        private readonly EntityVisualModelCatalog _entityVisualModelsManager;

        public UnitsControllerFactory(
            EntityUnitVariantsCatalog entityUnitsManager,
            EntityVisualModelCatalog entityVisualModelsManager)
        {
            _entityUnitsManager = entityUnitsManager;
            _entityVisualModelsManager = entityVisualModelsManager;
        }

        public UnitsController Create(GameObject parentGameObject, string entityVisualModelName, string variantName)
        {
            var entityVisualModel = _entityVisualModelsManager.GetEntityVisualModel(entityVisualModelName);
            if (entityVisualModel == null)
            {
                SLog.Error($"Cannot find entity visual model with name: {entityVisualModelName}");
                return null;
            }

            var entityUnitsRoot = new GameObject("Units");
            entityUnitsRoot.transform.parent = parentGameObject.transform;

            var animator = entityUnitsRoot.AddComponent<Animator>();
            animator.runtimeAnimatorController = entityVisualModel.Animator;
            var unitsController = new UnitsController(animator);

            foreach (var unitVisualModel in entityVisualModel.Units)
            {
                unitsController.AddUnit(CreateUnit(unitVisualModel, variantName));
            }

            // todo: maybe don't need this here
            unitsController.UpdateOrderInLayer();

            return unitsController;
        }

        private Unit CreateUnit(EntityUnitVisualModel entityUnitVisualModel, string variantName)
        {
            var unitVariants = _entityUnitsManager.GetUnit(entityUnitVisualModel.Path);
            if (unitVariants == null)
            {
                SLog.Error($"Cannot find unit variants by path: {entityUnitVisualModel.Path}");
                return null;
            }

            var unitVariant = unitVariants.GetVariant(variantName);
            if (unitVariant == null)
            {
                SLog.Error($"Cannot find unit variant by name: {variantName}");
                return null;
            }

            var entityUnitGameObject = new GameObject(entityUnitVisualModel.Path.ToString());
            entityUnitGameObject.AddComponent<SpriteRenderer>();
            var entityUnit = new Unit(entityUnitGameObject, entityUnitVisualModel.Path.ToString(), entityUnitVisualModel.RelativeOrderInLayer);

            entityUnit.Set(unitVariant.Sprite);

            return entityUnit;
        }
    }
}
