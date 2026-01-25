using Assets._Game.Scripts.ScriptableObjects;
using Assets.CoreScripts;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Entities
{
    public class EntityBuilder
    {
        private static int _entitiesCounter = 0;

        private readonly EntityUnitVariantsManager _entityUnitsManager;
        private readonly EntityVisualModelsManager _entityVisualModelsManager;

        public EntityBuilder(EntityUnitVariantsManager entityUnitsManager, EntityVisualModelsManager entityVisualModelsManager)
        {
            _entityUnitsManager = entityUnitsManager;
            _entityVisualModelsManager = entityVisualModelsManager;
        }

        public Entity Build(string entityName)
        {
            var entityVisualModel = _entityVisualModelsManager.Views.FirstOrDefault(x => x.Name == entityName);
            if (entityVisualModel == null)
            {
                SLog.Error($"Cannot find entity visual model with name: {entityName}");
                return null;
            }

            var entityGameObject = new GameObject($"{entityName} ({++_entitiesCounter})");
            var entityUnitsRoot = new GameObject("Units");
            entityUnitsRoot.transform.parent = entityGameObject.transform;
            var animator = entityUnitsRoot.AddComponent<Animator>();
            animator.runtimeAnimatorController = entityVisualModel.Animator;

            var unitsController = new EntityUnitsController(animator);
            var entity = new Entity(unitsController);

            foreach (var unitVisualModel in entityVisualModel.Units)
            {
                entity.UnitsController.AddUnit(BuildUnit(unitVisualModel));
            }

            return entity;
        }

        private EntityUnit BuildUnit(EntityUnitVisualModel entityUnitVisualModel)
        {
            var unitName = Path.GetFileName(entityUnitVisualModel.Path);
            var unitVariants = _entityUnitsManager.GetUnit(unitName);
            if (unitVariants == null)
            {
                SLog.Error($"Cannot find unit variants by name: {unitName}");
                return null;
            }

            var unitVariantName = "";
            var unitVariant = unitVariants.GetVariant(unitVariantName);
            if (unitVariant == null)
            {
                SLog.Error($"Cannot find unit variant by name: {unitVariantName}");
                return null;
            }

            var entityUnitGameObject = new GameObject(unitName);
            var spriteRenderer = entityUnitGameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = entityUnitVisualModel.OrderInLayer;
            var entityUnit = new EntityUnit(entityUnitGameObject, entityUnitVisualModel.Path);

            entityUnit.Set(unitVariant.Sprite);

            return entityUnit;
        }
    }
}
