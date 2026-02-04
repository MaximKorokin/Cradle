using Assets._Game.Scripts.Shared.Extensions;
using Assets.CoreScripts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Items.Inventory
{
    public sealed class InventoryModel : IItemContainer<int>
    {
        private readonly ItemStack[] _slots;

        public event Action Changed;

        public InventoryModel(int slotCount)
        {
            _slots = new ItemStack[slotCount];
        }

        public int SlotCount => _slots.Length;

        public IEnumerable<(int Index, ItemStack Stack)> Enumerate()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                yield return (i, _slots[i]);
            }
        }

        public ItemStack Get(int index)
        {
            if (!IsValidIndex(index))
            {
                SLog.Error($"Index {index} is out of range for inventory with {_slots.Length} slots.");
                return null;
            }

            return _slots[index];
        }

        public bool Contains(string id, int amount)
        {
            return _slots.Where(s => s.HasId(id)).Sum(s => s.Amount) >= amount;
        }

        public bool Contains(ItemStack item)
        {
            return _slots.Any(s => s == item);
        }

        public void Take(int index, ref int amount)
        {
            if (!IsValidIndex(index))
            {
                SLog.Error($"Index {index} is out of range for inventory with {_slots.Length} slots.");
                return;
            }

            var slot = _slots[index];
            if (slot == null)
            {
                SLog.Error($"No item in slot {index} to take from.");
                return;
            }

            slot.RemoveFrom(ref amount);

            // If the slot is empty after removal, set it to null
            if (slot.Amount <= 0)
            {
                _slots[index] = null;
            }

            Changed?.Invoke();
        }

        public void Take(string id, ref int amount)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                var slot = _slots[i];
                if (!slot.HasId(id)) continue;

                slot.RemoveFrom(ref amount);

                // If the slot is empty after removal, set it to null
                if (slot.Amount <= 0)
                {
                    _slots[i] = null;
                }
                if (amount <= 0)
                {
                    break;
                }
            }

            Changed?.Invoke();
        }

        public void Take(ItemStack item)
        {
            var itemIndex = Array.IndexOf(_slots, item);
            if (itemIndex == -1)
            {
                SLog.Error($"Item {item.Definition.Name} not found in inventory.");
                return;
            }
            _slots[itemIndex] = null;
        }

        public bool CanPut(int index, ItemStack item)
        {
            if (!IsValidIndex(index))
            {
                SLog.Error($"Index {index} is out of range for inventory with {_slots.Length} slots.");
                return false;
            }

            var slot = _slots[index];

            // Empty slot can accept any item
            if (slot == null) return true;

            return slot.CanAddTo(item);
        }

        public bool CanPut(ItemStack item)
        {
            // Empty or compatible slot found
            return _slots.Any(slot => slot == null || item.CanAddTo(slot));
        }

        public void Put(int index, ItemStack item)
        {
            if (!IsValidIndex(index))
            {
                SLog.Error($"Index {index} is out of range for inventory with {_slots.Length} slots.");
                return;
            }

            var slot = _slots[index];
            if (slot == null)
            {
                // Place the item in the empty slot
                _slots[index] = item;
            }
            else if (slot.CanAddTo(item))
            {
                // Add to existing stack
                item.AddTo(slot);
            }
            else
            {
                SLog.Error($"Cannot put item {item.Definition.Id} into slot {index}.");
            }

            Changed?.Invoke();
        }

        public void Put(ItemStack item)
        {
            // First try to stack into existing slots
            foreach (var slot in _slots.Where(s => s != null && item.CanAddTo(s)))
            {
                item.AddTo(slot);
                if (item.Amount <= 0) return;
            }

            // Then put into empty slot
            var indexOfEmpty = Array.IndexOf(_slots, null);
            if (indexOfEmpty >= 0)
            {
                _slots[indexOfEmpty] = item;
            }

            Changed?.Invoke();
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < _slots.Length;
        }
    }
}