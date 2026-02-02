using Assets._Game.Scripts.Entities.Controllers;
using Assets._Game.Scripts.Entities.Modules;
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
            var entity = Create(entityDefinition);
            if (entity == null)
            {
                SLog.Error($"Cannot create entity with id: {entityDefinition.EntityId}");
                return null;
            }
            if (entity.TryGetModule<EntityInventoryEquipmentModule>(out var containersController))
            {
                _inventoryEquipmentControllerAssembler.Apply(containersController, save);
            }
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
            var attributes = new EntityAttributesModule();
            var containersController = _inventoryEquipmentControllerAssembler.Create(entityDefinition);
            var entity = new Entity("");
            entity.AddModule(attributes);
            entity.AddModule(unitsController);
            entity.AddModule(behaviourController);
            entity.AddModule(containersController);

            foreach (var unitVisualModel in entityVisualModel.Units)
            {
                unitsController.AddUnit(CreateUnit(unitVisualModel, entityVisualModel.Variant));
            }

            unitsController.UpdateOrderInLayer();

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
