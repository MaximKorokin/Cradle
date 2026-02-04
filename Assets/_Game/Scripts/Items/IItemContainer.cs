using System.Collections.Generic;

namespace Assets._Game.Scripts.Items
{
    public interface IItemContainer<T> : IItemContainer
    {
        IEnumerable<(T Index, ItemStack Stack)> Enumerate();

        ItemStack Get(T index);
        void Take(T index, ref int amount);
        bool CanPut(T index, ItemStack item);
        void Put(T index, ItemStack item);
    }

    public interface IItemContainer
    {
        event System.Action Changed;

        int SlotCount { get; }
        bool Contains(string id, int amount);
        bool Contains(ItemStack item);
        void Take(string id, ref int amount);
        void Take(ItemStack item);
        bool CanPut(ItemStack item);
        void Put(ItemStack item);
    }
}
