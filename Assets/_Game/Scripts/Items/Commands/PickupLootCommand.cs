using Assets._Game.Scripts.Items.Inventory;

namespace Assets._Game.Scripts.Items.Commands
{
    internal class PickupLootCommand : IItemCommand
    {
        public InventoryModel InventoryModel;
        public ItemStack ItemStack;
    }
}
