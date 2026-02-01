using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;

namespace Assets._Game.Scripts.Items
{
    public class InventoryEquipmentControllerAssembler
    {
        private readonly InventoryModelAssembler _inventoryModelAssembler;
        private readonly EquipmentModelAssembler _equipmentModelAssembler;

        public InventoryEquipmentControllerAssembler(InventoryModelAssembler inventoryModelAssembler, EquipmentModelAssembler equipmentModelAssembler)
        {
            _inventoryModelAssembler = inventoryModelAssembler;
            _equipmentModelAssembler = equipmentModelAssembler;
        }

        public InventoryEquipmentController Assemble(EntityDefinition entityDefinition, EntitySave entitySave)
        {
            var inventoryModel = _inventoryModelAssembler.Create(entityDefinition);
            _inventoryModelAssembler.Apply(inventoryModel, entitySave.InventorySave);

            var equipmentModel = _equipmentModelAssembler.Create(entityDefinition);
            _equipmentModelAssembler.Apply(equipmentModel, entitySave.EquipmentSave);
            return new InventoryEquipmentController(inventoryModel, equipmentModel);
        }

        public InventoryEquipmentController Create(EntityDefinition entityDefinition)
        {
            var inventoryModel = _inventoryModelAssembler.Create(entityDefinition);
            var equipmentModel = _equipmentModelAssembler.Create(entityDefinition);
            return new InventoryEquipmentController(inventoryModel, equipmentModel);
        }
    }
}
