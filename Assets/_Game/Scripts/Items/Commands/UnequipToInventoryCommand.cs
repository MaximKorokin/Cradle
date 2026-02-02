using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;

namespace Assets._Game.Scripts.Items.Commands
{
    public class UnequipToInventoryCommand : IItemCommand
    {
        public InventoryModel InventoryModel;
        public EquipmentModel EquipmentModel;
        public EquipmentSlotType SlotType;
    }
}
