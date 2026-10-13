namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct AddItemCommand : IItemCommand
    {
        public readonly ItemContainerPath ToContainer;
        public readonly ItemStackSnapshot Item;

        public AddItemCommand(ItemContainerPath toContainer, ItemStackSnapshot item)
        {
            ToContainer = toContainer;
            Item = item;
        }
    }
}
