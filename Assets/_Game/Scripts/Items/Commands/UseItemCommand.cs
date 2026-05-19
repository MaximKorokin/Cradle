namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct UseItemCommand : IItemCommand
    {
        public readonly ItemContainerPath Container;
        public readonly long Slot;
        public readonly bool IsManual;

        public UseItemCommand(ItemContainerPath container, long slot, bool isManual)
        {
            Container = container;
            Slot = slot;
            IsManual = isManual;
        }
    }
}
