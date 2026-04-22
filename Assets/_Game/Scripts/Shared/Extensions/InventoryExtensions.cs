using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Inventory;
using System;
using System.Linq;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class InventoryExtensions
    {
        private static void SortBy(this InventoryModel inventoryModel, Func<ItemStackSnapshot, object> keySelector)
        {
            var items = inventoryModel.Enumerate()
                .Where(x => x.Snapshot != null)
                .OrderBy(x => keySelector(x.Snapshot.Value))
                .ToList();

            // Clear all slots first
            foreach (var (slot, snapshot) in inventoryModel.Enumerate())
            {
                if (snapshot != null)
                {
                    inventoryModel.RemoveFromSlot(slot, snapshot.Value.Amount);
                }
            }

            // Add items back in sorted order
            foreach (var (_, snapshot) in items)
            {
                inventoryModel.Add(snapshot.Value);
            }
        }

        public static void SortByName(this InventoryModel inventoryModel)
        {
            inventoryModel.SortBy(x => x.Definition.Name);
        }

        public static void SortByPurpose(this InventoryModel inventoryModel)
        {
            inventoryModel.SortBy(x => x.GetPurpose());
        }
    }
}
