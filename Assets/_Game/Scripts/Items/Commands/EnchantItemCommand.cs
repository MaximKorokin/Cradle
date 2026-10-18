namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct EnchantItemCommand : IItemCommand
    {
        public readonly ItemStackPath EnchantingItem;
        public readonly ItemStackPath EnchantableItem;
        
        public EnchantItemCommand(ItemStackPath enchantingItem, ItemStackPath enchantableItem)
        {
            EnchantingItem = enchantingItem;
            EnchantableItem = enchantableItem;
        }
    }
}
