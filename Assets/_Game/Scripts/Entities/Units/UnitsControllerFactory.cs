using Assets._Game.Scripts.ScriptableObjectManagers;
using Assets.CoreScripts;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public sealed class UnitsControllerFactory
    {
        private readonly EntityUnitVariantsManager _entityUnitsManager;
        private readonly EntityVisualModelsManager _entityVisualModelsManager;

        public UnitsControllerFactory(
            EntityUnitVariantsManager entityUnitsManager,
            EntityVisualModelsManager entityVisualModelsManager)
        {
            _entityUnitsManager = entityUnitsManager;
            _entityVisualModelsManager = entityVisualModelsManager;
        }

        public UnitsController Create(GameObject parentGameObject, string entityName)
        {
            var entityVisualModel = _entityVisualModelsManager.Views.FirstOrDefault(x => x.Name == entityName);
            if (entityVisualModel == null)
            {
                SLog.Error($"Cannot find entity visual model with name: {entityName}");
                return null;
            }

            var entityUnitsRoot = new GameObject("Units");
            entityUnitsRoot.transform.parent = parentGameObject.transform;

            var animator = entityUnitsRoot.AddComponent<Animator>();
            animator.runtimeAnimatorController = entityVisualModel.Animator;
            var unitsController = new UnitsController(animator);

            foreach (var unitVisualModel in entityVisualModel.Units)
            {
                unitsController.AddUnit(CreateUnit(unitVisualModel, entityVisualModel.Variant));
            }

            // todo: maybe don't need this here
            unitsController.UpdateOrderInLayer();

            return unitsController;
        }

        private Unit CreateUnit(EntityUnitVisualModel entityUnitVisualModel, string variantName)
        {
            var unitName = Path.GetFileName(entityUnitVisualModel.Path);
            var unitVariants = _entityUnitsManager.GetUnit(unitName);
            if (unitVariants == null)
            {
                SLog.Error($"Cannot find unit variants by name: {unitName}");
                return null;
            }

            var unitVariant = unitVariants.GetVariant(variantName);
            if (unitVariant == null)
            {
                SLog.Error($"Cannot find unit variant by name: {variantName}");
                return null;
            }

            var entityUnitGameObject = new GameObject(unitName);
            var spriteRenderer = entityUnitGameObject.AddComponent<SpriteRenderer>();
            var entityUnit = new Unit(entityUnitGameObject, entityUnitVisualModel.Path, entityUnitVisualModel.RelativeOrderInLayer);

            entityUnit.Set(unitVariant.Sprite);

            return entityUnit;
        }
    }
}
