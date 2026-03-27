namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct MoveItemCommand<T> : IItemCommand
    {
        public readonly IItemContainer<T> FromContainer;
        public readonly T FromSlot;
        public readonly IItemContainer ToContainer;
        public readonly int Amount;

        public MoveItemCommand(IItemContainer<T> fromContainer, T fromSlot, IItemContainer toContainer, int amount)
        {
            FromContainer = fromContainer;
            FromSlot = fromSlot;
            ToContainer = toContainer;
            Amount = amount;
        }
    }
}
