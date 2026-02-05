using Assets._Game.Scripts.Shared.Extensions;
using Assets.CoreScripts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Items.Equipment
{
    public class EquipmentModel : IItemContainer<EquipmentSlotKey>
    {
        private readonly Dictionary<EquipmentSlotKey, ItemStack> _slots;

        public EquipmentModel(EquipmentSlotType[] slots)
        {
            _slots = new();
            var slotTypeCounts = new Dictionary<EquipmentSlotType, int>();
            foreach (var slotType in slots)
            {
                var count = 0;
                if (slotTypeCounts.TryGetValue(slotType, out var slotTypeCount))
                    count = slotTypeCount;
                _slots[new(slotType, count)] = null;
                slotTypeCounts[slotType] = count + 1;
            }
        }

        public int SlotCount => _slots.Count;

        public event Action Changed;

        public bool CanPut(EquipmentSlotKey key, ItemStack item) => CanPut(key, item, false);
        public bool CanPut(EquipmentSlotKey key, ItemStack item, bool ignoreItemInSlot)
        {
            // todo: lvl, stats etc. requirements

            if (!IsValidKey(key) || key.SlotType != item.GetEquipmentSlotType()) return false;

            if (ignoreItemInSlot) return true;

            // slot is empty or can stack
            var toItem = _slots[key];
            return toItem == null || item.CanAddTo(toItem);
        }

        public bool CanPut(ItemStack item) => CanPut(item, false);
        public bool CanPut(ItemStack item, bool ignoreItemInSlot)
        {
            // can't put null
            return item != null && _slots.Any(s => CanPut(s.Key, item, ignoreItemInSlot));
        }

        public bool Contains(string id, int amount)
        {
            return _slots.Values.Where(item => item.HasId(id)).Sum(item => item.Amount) >= amount;
        }

        public bool Contains(ItemStack item)
        {
            // can't contain null
            return item != null && _slots.Values.Any(equipmentItem => equipmentItem == item);
        }

        public IEnumerable<(EquipmentSlotKey Index, ItemStack Stack)> Enumerate()
        {
            foreach (var slot in _slots)
            {
                yield return (slot.Key, slot.Value);
            }
        }

        public ItemStack Get(EquipmentSlotKey slot)
        {
            return IsValidKey(slot) && _slots.TryGetValue(slot, out var item) ? item : null;
        }

        public void Put(EquipmentSlotKey slot, ItemStack item)
        {
            if (!CanPut(slot, item))
            {
                SLog.Error($"Cannot put item {item.Definition.Id} into equipment slot {slot}");
                return;
            }

            var toItem = _slots[slot];
            if (toItem == null)
            {
                // Empty slot, just put it in
                _slots[slot] = item;
            }
            else
            {
                // Stack items
                item.AddTo(toItem);
            }

            Changed?.Invoke();
        }

        public void Put(ItemStack item)
        {
            var putSome = false;
            var slotType = item.GetEquipmentSlotType();
            foreach (var slot in _slots.Where(s => s.Key.SlotType == slotType))
            {
                if (CanPut(slot.Key, item))
                {
                    putSome = true;
                    var toItem = _slots[slot.Key];
                    if (toItem == null)
                    {
                        // Empty slot, just put it in
                        _slots[slot.Key] = item;
                        break;
                    }
                    else
                    {
                        // Stack items
                        item.AddTo(toItem);
                    }
                    // Item amount exhausted
                    if (item.Amount <= 0) break;
                }
            }

            if (!putSome) SLog.Error($"Cannot put item {item.Definition.Id} into equipment");
            else Changed?.Invoke();
        }

        public void Take(EquipmentSlotKey slot, ref int amount)
        {
            if (!IsValidKey(slot) || _slots[slot] == null)
            {
                SLog.Error($"Cannot take from slot {slot}");
                return;
            }

            var fromItem = _slots[slot];
            fromItem.RemoveFrom(ref amount);
            if (fromItem.Amount <= 0) _slots[slot] = null;

            Changed?.Invoke();
        }

        public void Take(string id, ref int amount)
        {
            // Remove from slots until amount is satisfied
            foreach (var slot in _slots.Where(slot => slot.Value.HasId(id)))
            {
                if (amount <= 0) break;
                slot.Value.RemoveFrom(ref amount);
                // Clean up empty slots
                if (slot.Value.Amount <= 0) _slots[slot.Key] = null;
            }

            Changed?.Invoke();
        }

        public void Take(ItemStack item)
        {
            var slot = _slots.FirstOrDefault(s => s.Value == item);
            if (slot.Value == null)
            {
                SLog.Error($"Item {item.Definition.Name} not found in equipment.");
                return;
            }
            _slots[slot.Key] = null;

            Changed?.Invoke();
        }

        public bool TryGetSlot(EquipmentSlotType slotType, Func<ItemStack, bool> predicate, out EquipmentSlotKey key)
        {
            key = _slots.FirstOrDefault(s => s.Key.SlotType == slotType && predicate(s.Value)).Key;
            return !key.Equals(default(EquipmentSlotKey));
        }

        private bool IsValidKey(EquipmentSlotKey key)
        {
            return _slots.ContainsKey(key);
        }
    }
}
