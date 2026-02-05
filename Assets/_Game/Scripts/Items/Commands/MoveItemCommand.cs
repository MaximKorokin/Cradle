namespace Assets._Game.Scripts.Items.Commands
{
    public sealed class MoveItemCommand<T> : IItemCommand
    {
        public IItemContainer<T> FromContainer;
        public T FromSlot;
        public IItemContainer ToContainer;
        public int Amount;
    }
}
