namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct TransferToContainerSlotCommand : IItemCommand
    {
        public readonly ItemContainerPath FromContainer;
        public readonly long FromSlot;
        public readonly ItemContainerPath ToContainer;
        public readonly long ToSlot;
        public readonly int Amount;

        public TransferToContainerSlotCommand(ItemContainerPath fromContainer, long fromSlot, ItemContainerPath toContainer, long toSlot, int amount)
        {
            FromContainer = fromContainer;
            FromSlot = fromSlot;
            ToSlot = toSlot;
            ToContainer = toContainer;
            Amount = amount;
        }
    }
}
