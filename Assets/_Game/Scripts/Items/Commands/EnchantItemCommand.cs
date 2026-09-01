namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct EnchantItemCommand : IItemCommand
    {
        public readonly ItemContainerPath FromContainer;
        public readonly long FromSlot;

        public EnchantItemCommand(ItemContainerPath fromContainer, long fromSlot)
        {
            FromContainer = fromContainer;
            FromSlot = fromSlot;
        }
    }
}
