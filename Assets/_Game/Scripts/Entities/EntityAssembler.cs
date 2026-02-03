using Assets._Game.Scripts.Entities.Controllers;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items;
using Assets.CoreScripts;
using UnityEngine;

namespace Assets._Game.Scripts.Entities
{
    public class EntityAssembler
    {
        private static int _entitiesCounter = 0;

        private readonly InventoryEquipmentControllerAssembler _inventoryEquipmentControllerAssembler;
        private readonly UnitsControllerFactory _unitsControllerFactory;

        public EntityAssembler(
            InventoryEquipmentControllerAssembler inventoryEquipmentControllerAssembler,
            UnitsControllerFactory unitsControllerFactory)
        {
            _inventoryEquipmentControllerAssembler = inventoryEquipmentControllerAssembler;
            _unitsControllerFactory = unitsControllerFactory;
        }

        public Entity Assemble(EntityDefinition entityDefinition, EntitySave save)
        {
            var entity = Create(entityDefinition);
            if (entity == null)
            {
                SLog.Error($"Cannot create entity with id: {entityDefinition.EntityId}");
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
            var entityGameObject = new GameObject($"{entityDefinition.EntityId} ({++_entitiesCounter})");

            var entity = new Entity("");
            entity.AddModule(new EntityAttributesModule());
            entity.AddModule(_unitsControllerFactory.Create(entityGameObject, entityDefinition.EntityId));
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
