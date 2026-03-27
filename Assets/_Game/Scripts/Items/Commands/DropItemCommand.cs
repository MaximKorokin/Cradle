namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct DropItemCommand<T> : IItemCommand
    {
        public readonly IItemContainer<T> FromContainer;
        public readonly T FromSlot;
        public readonly int Amount;

        public DropItemCommand(IItemContainer<T> fromContainer, T fromSlot, int amount)
        {
            FromContainer = fromContainer;
            FromSlot = fromSlot;
            Amount = amount;
        }
    }
}
