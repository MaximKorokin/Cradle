using Assets._Game.Scripts.Items.Inventory;

namespace Assets._Game.Scripts.Items.Commands
{
    public sealed class MoveItemToSlotCommand : IItemCommand
    {
        public InventoryModel From;
        public int FromSlot;
        public InventoryModel To;
        public int ToSlot;
        public int Amount;
    }
}
