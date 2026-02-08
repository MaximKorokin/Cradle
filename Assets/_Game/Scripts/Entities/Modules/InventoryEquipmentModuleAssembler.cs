using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;

namespace Assets._Game.Scripts.Entities.Modules
{
    public class InventoryEquipmentModuleAssembler
    {
        private readonly InventoryModelAssembler _inventoryModelAssembler;
        private readonly EquipmentModelAssembler _equipmentModelAssembler;

        public InventoryEquipmentModuleAssembler(InventoryModelAssembler inventoryModelAssembler, EquipmentModelAssembler equipmentModelAssembler)
        {
            _inventoryModelAssembler = inventoryModelAssembler;
            _equipmentModelAssembler = equipmentModelAssembler;
        }

        public InventoryEquipmentModule Apply(InventoryEquipmentModule entityInventoryEquipmentModule, EntitySave entitySave)
        {
            _inventoryModelAssembler.Apply(entityInventoryEquipmentModule.Inventory, entitySave.InventorySave);
            _equipmentModelAssembler.Apply(entityInventoryEquipmentModule.Equipment, entitySave.EquipmentSave);
            return entityInventoryEquipmentModule;
        }

        public InventoryEquipmentModule Create(EntityDefinition entityDefinition)
        {
            var inventoryModel = _inventoryModelAssembler.Create(entityDefinition);
            var equipmentModel = _equipmentModelAssembler.Create(entityDefinition);
            return new InventoryEquipmentModule(inventoryModel, equipmentModel);
        }

        public (InventorySave InventorySave, EquipmentSave EquipmentSave) Save(InventoryEquipmentModule entityInventoryEquipmentModule)
        {
            var inventorySave = _inventoryModelAssembler.Save(entityInventoryEquipmentModule.Inventory);
            var equipmentSave = _equipmentModelAssembler.Save(entityInventoryEquipmentModule.Equipment);
            return (inventorySave, equipmentSave);
        }
    }
}
