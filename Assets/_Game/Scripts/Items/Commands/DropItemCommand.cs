namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct DropItemCommand : IItemCommand
    {
        public readonly ItemContainerPath FromContainer;
        public readonly long FromSlot;
        public readonly int Amount;

        public DropItemCommand(ItemContainerPath fromContainer, long fromSlot, int amount)
        {
            FromContainer = fromContainer;
            FromSlot = fromSlot;
            Amount = amount;
        }
    }
}
