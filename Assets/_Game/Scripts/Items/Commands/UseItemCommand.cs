namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct UseItemCommand : IItemCommand
    {
        public readonly ItemContainerId Container;
        public readonly long Slot;
        public readonly bool IsManual;

        public UseItemCommand(ItemContainerId container, long slot, bool isManual)
        {
            Container = container;
            Slot = slot;
            IsManual = isManual;
        }
    }
}
