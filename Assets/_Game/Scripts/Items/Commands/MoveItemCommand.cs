namespace Assets._Game.Scripts.Items.Commands
{
    public sealed class MoveItemCommand : IItemCommand
    {
        public IItemContainer FromContainer;
        public ItemStack FromItem;
        public IItemContainer ToContainer;
        public int Amount;
    }
}
