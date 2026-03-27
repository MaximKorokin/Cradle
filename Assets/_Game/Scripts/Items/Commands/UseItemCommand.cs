namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct UseItemCommand : IItemCommand
    {
        public readonly ItemContainerId Container;
        public readonly long Slot;

        public UseItemCommand(ItemContainerId container, long slot)
        {
            Container = container;
            Slot = slot;
        }
    }
}
