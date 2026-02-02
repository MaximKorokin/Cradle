using System.Collections.Generic;

namespace Assets._Game.Scripts.Items
{
    public interface IItemContainer<T> : IItemContainer
    {
        event System.Action Changed;

        IEnumerable<(T index, ItemStack stack)> Enumerate();

        ItemStack Get(T index);
        void Take(T index, ref int amount);
        bool CanPut(T index, ItemStack item);
        void Put(T index, ItemStack item);
    }

    public interface IItemContainer
    {
        int SlotCount { get; }
        bool Contains(string id, int amount);
        void Take(string id, ref int amount);
        bool CanPut(ItemStack item);
        void Put(ItemStack item);
    }
}
