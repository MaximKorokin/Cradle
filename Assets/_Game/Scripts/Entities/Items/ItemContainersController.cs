using Assets._Game.Scripts.Entities.Items.Equipment;
using Assets._Game.Scripts.Entities.Items.Inventory;

namespace Assets._Game.Scripts.Entities.Items
{
    public class ItemContainersController
    {
        public ItemContainersController(InventoryModel inventory, EquipmentModel equipment)
        {
            Inventory = inventory;
            Equipment = equipment;
        }

        public InventoryModel Inventory { get; private set; }
        public EquipmentModel Equipment { get; private set; }

        public bool TryEquip(int inventorySlot, EquipmentSlotType slot)
        {
            return false;
        }

        public bool TryUnequip(EquipmentSlotType slot)
        {
            return false;
        }

        public bool Move(IItemContainer fromContainer, int fromSlot, IItemContainer toContainer, int toSlot)
        {
            return false;
        }
    }
}
