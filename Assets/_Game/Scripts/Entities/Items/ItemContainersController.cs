using Assets._Game.Scripts.Entities.Items.Equipment;

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

        public bool Move<T1, T2>(IItemContainer<T1> fromContainer, T1 fromSlot, IItemContainer<T2> toContainer, T2 toSlot)
        {
            return false;
        }
    }
}
