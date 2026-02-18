using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using System.Linq;

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
            InventoryModel inventoryModel = null;
            if (entityDefinition.TryGetModule<InventoryModuleDefinition>(out var inventoryDefinitionModule))
            {
                inventoryModel = _inventoryModelAssembler.Create(inventoryDefinitionModule.SlotsAmount);
            }

            EquipmentModel equipmentModel = null;
            if (entityDefinition.TryGetModule<EquipmentModuleDefinition>(out var equipmentDefinitionModule))
            {
                var slots = equipmentDefinitionModule.EquipmentSlots.ToArray();
                equipmentModel = _equipmentModelAssembler.Create(slots);
            }

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
