namespace Assets._Game.Scripts.Entities.Items
{
    public interface IItemContainer
    {
        int SlotCount { get; }
        ItemStack Get(int slot);
        bool CanPut(int slot, ItemStack stack);
        void Put(int slot, ItemStack stack);
        ItemStack Take(int slot, int amount);
    }
}
