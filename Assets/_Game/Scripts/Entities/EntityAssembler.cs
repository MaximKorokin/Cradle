using Assets._Game.Scripts.Entities.Controllers;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.ScriptableObjectManagers;
using Assets.CoreScripts;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Entities
{
    public class EntityAssembler
    {
        private static int _entitiesCounter = 0;

        private readonly EntityUnitVariantsManager _entityUnitsManager;
        private readonly EntityVisualModelsManager _entityVisualModelsManager;

        private readonly InventoryEquipmentControllerAssembler _inventoryEquipmentControllerAssembler;

        public EntityAssembler(
            EntityUnitVariantsManager entityUnitsManager,
            EntityVisualModelsManager entityVisualModelsManager,
            InventoryEquipmentControllerAssembler inventoryEquipmentControllerAssembler)
        {
            _entityUnitsManager = entityUnitsManager;
            _entityVisualModelsManager = entityVisualModelsManager;

            _inventoryEquipmentControllerAssembler = inventoryEquipmentControllerAssembler;
        }

        public Entity Assemble(EntityDefinition entityDefinition, EntitySave save)
        {
            var entityVisualModel = _entityVisualModelsManager.Views.FirstOrDefault(x => x.Name == save.EntityId);
            if (entityVisualModel == null)
            {
                SLog.Error($"Cannot find entity visual model with name: {save.EntityId}");
                return null;
            }

            var entityGameObject = new GameObject($"{save.EntityId} ({++_entitiesCounter})");
            var entityUnitsRoot = new GameObject("Units");
            entityUnitsRoot.transform.parent = entityGameObject.transform;

            var animator = entityUnitsRoot.AddComponent<Animator>();
            animator.runtimeAnimatorController = entityVisualModel.Animator;
            var unitsController = new UnitsController(animator);
            var behaviourController = new BehaviourController();
            var attributes = new EntityAttributesModel();
            var containersController = _inventoryEquipmentControllerAssembler.Assemble(entityDefinition, save);
            var entity = new Entity("", null, unitsController, behaviourController, containersController);

            foreach (var unitVisualModel in entityVisualModel.Units)
            {
                entity.UnitsController.AddUnit(CreateUnit(unitVisualModel, entityVisualModel.Variant));
            }

            entity.UnitsController.UpdateOrderInLayer();

            return entity;
        }

        public Entity Create(EntityDefinition entityDefinition)
        {
            var entityVisualModel = _entityVisualModelsManager.Views.FirstOrDefault(x => x.Name == entityDefinition.EntityId);
            if (entityVisualModel == null)
            {
                SLog.Error($"Cannot find entity visual model with name: {entityDefinition.EntityId}");
                return null;
            }

            var entityGameObject = new GameObject($"{entityDefinition.EntityId} ({++_entitiesCounter})");
            var entityUnitsRoot = new GameObject("Units");
            entityUnitsRoot.transform.parent = entityGameObject.transform;

            var animator = entityUnitsRoot.AddComponent<Animator>();
            animator.runtimeAnimatorController = entityVisualModel.Animator;
            var unitsController = new UnitsController(animator);
            var behaviourController = new BehaviourController();
            var attributes = new EntityAttributesModel();
            var containersController = _inventoryEquipmentControllerAssembler.Create(entityDefinition);
            var entity = new Entity("", null, unitsController, behaviourController, containersController);

            foreach (var unitVisualModel in entityVisualModel.Units)
            {
                entity.UnitsController.AddUnit(CreateUnit(unitVisualModel, entityVisualModel.Variant));
            }

            entity.UnitsController.UpdateOrderInLayer();

            return entity;
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
