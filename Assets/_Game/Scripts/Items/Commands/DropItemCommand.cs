namespace Assets._Game.Scripts.Items.Commands
{
    public sealed class DropItemCommand<T> : IItemCommand
    {
        public IItemContainer<T> FromContainer;
        public T FromSlot;
        public int Amount;
    }
}
