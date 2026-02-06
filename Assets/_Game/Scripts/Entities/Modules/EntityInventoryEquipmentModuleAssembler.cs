using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;

namespace Assets._Game.Scripts.Entities.Modules
{
    public class EntityInventoryEquipmentModuleAssembler
    {
        private readonly InventoryModelAssembler _inventoryModelAssembler;
        private readonly EquipmentModelAssembler _equipmentModelAssembler;

        public EntityInventoryEquipmentModuleAssembler(InventoryModelAssembler inventoryModelAssembler, EquipmentModelAssembler equipmentModelAssembler)
        {
            _inventoryModelAssembler = inventoryModelAssembler;
            _equipmentModelAssembler = equipmentModelAssembler;
        }

        public EntityInventoryEquipmentModule Apply(EntityInventoryEquipmentModule entityInventoryEquipmentModule, EntitySave entitySave)
        {
            _inventoryModelAssembler.Apply(entityInventoryEquipmentModule.Inventory, entitySave.InventorySave);
            _equipmentModelAssembler.Apply(entityInventoryEquipmentModule.Equipment, entitySave.EquipmentSave);
            return entityInventoryEquipmentModule;
        }

        public EntityInventoryEquipmentModule Create(EntityDefinition entityDefinition)
        {
            var inventoryModel = _inventoryModelAssembler.Create(entityDefinition);
            var equipmentModel = _equipmentModelAssembler.Create(entityDefinition);
            return new EntityInventoryEquipmentModule(inventoryModel, equipmentModel);
        }

        public (InventorySave InventorySave, EquipmentSave EquipmentSave) Save(EntityInventoryEquipmentModule entityInventoryEquipmentModule)
        {
            var inventorySave = _inventoryModelAssembler.Save(entityInventoryEquipmentModule.Inventory);
            var equipmentSave = _equipmentModelAssembler.Save(entityInventoryEquipmentModule.Equipment);
            return (inventorySave, equipmentSave);
        }
    }
}
