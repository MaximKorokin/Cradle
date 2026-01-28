using Assets._Game.Scripts.Entities.Controllers;
using Assets._Game.Scripts.Entities.Items;
using Assets._Game.Scripts.Entities.Items.Inventory;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.ScriptableObjectManagers;
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
            var unitsController = new UnitsController(animator);
            var behaviourController = new BehaviourController();
            var attributes = new Attributes();
            var containersController = new ItemContainersController(new(), new());
            var entity = new Entity(unitsController, behaviourController, attributes, containersController);

            foreach (var unitVisualModel in entityVisualModel.Units)
            {
                entity.UnitsController.AddUnit(BuildUnit(unitVisualModel, entityVisualModel.Variant));
            }

            entity.UnitsController.UpdateOrderInLayer();

            return entity;
        }

        private Unit BuildUnit(EntityUnitVisualModel entityUnitVisualModel, string variantName)
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
