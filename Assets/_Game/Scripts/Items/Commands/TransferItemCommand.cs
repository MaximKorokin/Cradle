namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct TransferItemCommand : IItemCommand
    {
        public readonly ItemContainerId FromContainer;
        public readonly long FromSlot;
        public readonly ItemContainerId ToContainer;
        public readonly int Amount;

        public TransferItemCommand(ItemContainerId fromContainer, long fromSlot, ItemContainerId toContainer, int amount)
        {
            FromContainer = fromContainer;
            FromSlot = fromSlot;
            ToContainer = toContainer;
            Amount = amount;
        }
    }
}
