namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct DestroyItemCommand : IItemCommand
    {
        public readonly ItemContainerPath FromContainer;
        public readonly long FromSlot;
        public readonly string ItemId;
        public readonly int Amount;

        public readonly bool HasSlot;

        public DestroyItemCommand(ItemContainerPath fromContainer, string itemId, int amount)
        {
            FromContainer = fromContainer;
            ItemId = itemId;
            Amount = amount;

            FromSlot = 0;
            HasSlot = false;
        }

        public DestroyItemCommand(ItemContainerPath fromContainer, long fromSlot, int amount)
        {
            FromContainer = fromContainer;
            FromSlot = fromSlot;
            Amount = amount;

            ItemId = null;
            HasSlot = true;
        }
    }
}
