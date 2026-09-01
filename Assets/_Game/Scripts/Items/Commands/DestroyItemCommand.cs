namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct DestroyItemCommand : IItemCommand
    {
        public readonly ItemContainerPath FromContainer;
        public readonly long FromSlot;
        public readonly int Amount;

        public DestroyItemCommand(ItemContainerPath fromContainer, long fromSlot, int amount)
        {
            FromContainer = fromContainer;
            FromSlot = fromSlot;
            Amount = amount;
        }
    }
}
