namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct TransferItemCommand : IItemCommand
    {
        public readonly ItemContainerPath FromContainer;
        public readonly long FromSlot;
        public readonly ItemContainerPath ToContainer;
        public readonly int Amount;

        public TransferItemCommand(ItemContainerPath fromContainer, long fromSlot, ItemContainerPath toContainer, int amount)
        {
            FromContainer = fromContainer;
            FromSlot = fromSlot;
            ToContainer = toContainer;
            Amount = amount;
        }
    }
}
