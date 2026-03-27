namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct UseItemCommand<T> : IItemCommand
    {
        public readonly IItemContainer<T> Container;
        public readonly T Slot;

        public UseItemCommand(IItemContainer<T> container, T slot)
        {
            Container = container;
            Slot = slot;
        }
    }
}
