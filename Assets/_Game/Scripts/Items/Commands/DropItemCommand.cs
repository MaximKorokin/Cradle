namespace Assets._Game.Scripts.Items.Commands
{
    public sealed class DropItemCommand : IItemCommand
    {
        public ItemStack Item;
        public IItemContainer FromContainer;
        public int Amount;
    }
}
