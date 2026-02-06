using Assets._Game.Scripts.Entities.Controllers;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items;
using Assets.CoreScripts;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.Entities
{
    public class EntityAssembler
    {
        private static int _entitiesCounter = 0;

        private readonly IObjectResolver _resolver;
        private readonly InventoryEquipmentControllerAssembler _inventoryEquipmentControllerAssembler;
        private readonly AppearanceModuleFactory _appearanceModuleFactory;

        public EntityAssembler(
            IObjectResolver resolver,
            InventoryEquipmentControllerAssembler inventoryEquipmentControllerAssembler,
            AppearanceModuleFactory appearanceModuleFactory)
        {
            _resolver = resolver;
            _inventoryEquipmentControllerAssembler = inventoryEquipmentControllerAssembler;
            _appearanceModuleFactory = appearanceModuleFactory;
        }

        public Entity Assemble(EntityDefinition entityDefinition, EntitySave save)
        {
            var entity = Create(entityDefinition);
            if (entity == null)
            {
                SLog.Error($"Cannot create entity with id: {entityDefinition.VisualModel}");
                return null;
            }
            if (entity.TryGetModule<EntityInventoryEquipmentModule>(out var inventoryEquipmentModule))
            {
                _inventoryEquipmentControllerAssembler.Apply(inventoryEquipmentModule, save);
            }
            return entity;
        }

        public Entity Create(EntityDefinition entityDefinition)
        {
            var entityView = _resolver.Instantiate(entityDefinition.VisualModel.BasePrefab);
            entityView.name = $"{entityDefinition.VisualModel} ({++_entitiesCounter})";
            entityView.UnitsAnimator.runtimeAnimatorController = entityDefinition.VisualModel.Animator;

            var entity = new Entity("");
            entity.AddModule(new EntityAttributesModule());
            entity.AddModule(_appearanceModuleFactory.Create(entityView, entityDefinition));
            entity.AddModule(new BehaviourController());
            entity.AddModule(_inventoryEquipmentControllerAssembler.Create(entityDefinition));

            return entity;
        }

        public EntitySave Save(Entity entity)
        {
            var save = new EntitySave();
            if (entity.TryGetModule<EntityInventoryEquipmentModule>(out var inventoryEquipmentModule))
            {
                var inventoryEquipmentSave = _inventoryEquipmentControllerAssembler.Save(inventoryEquipmentModule);
                save.InventorySave = inventoryEquipmentSave.InventorySave;
                save.EquipmentSave = inventoryEquipmentSave.EquipmentSave;
            }
            return save;
        }
    }
}
