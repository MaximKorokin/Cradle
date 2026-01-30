namespace Assets._Game.Scripts.Items
{
    public interface IItemContainer<T>
    {
        int SlotCount { get; }

        ItemStack Get(T index);

        bool Contains(string id, int amount);

        void Take(T index, ref int amount);
        void Take(string id, ref int amount);

        bool CanPut(T index, ItemStack item);
        bool CanPut(ItemStack item);

        void Put(T index, ItemStack item);
        void Put(ItemStack item);
    }
}
