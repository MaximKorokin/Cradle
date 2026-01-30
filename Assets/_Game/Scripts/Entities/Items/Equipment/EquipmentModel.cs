using Assets._Game.Scripts.Shared.Extensions;
using Assets.CoreScripts;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Items.Equipment
{
    public class EquipmentModel : IItemContainer<EquipmentSlotType>
    {
        private readonly EquipmentSlot[] _slots;

        public EquipmentModel(EquipmentSlot[] slots)
        {
            _slots = slots;
        }

        public int SlotCount => _slots.Length;

        public bool CanPut(EquipmentSlotType index, ItemStack item)
        {
            // todo: lvl, stats etc. requirements

            if (index != item.GetEquipmentSlotType()) return false;

            // slot is empty or can stack
            return GetSlots(index).Any(slot => slot.Item == null || item.CanAddTo(slot.Item));
        }

        public bool CanPut(ItemStack item)
        {
            return CanPut(item.GetEquipmentSlotType(), item);
        }

        public bool Contains(string id, int amount)
        {
            return _slots.Where(s => s.Item.HasId(id)).Sum(s => s.Item.Amount) >= amount;
        }

        public ItemStack Get(EquipmentSlotType index)
        {
            return GetSlots(index).FirstOrDefault()?.Item;
        }

        public void Put(EquipmentSlotType index, ItemStack item)
        {
            if (!CanPut(index, item))
            {
                SLog.Error($"Cannot put item {item.Definition.Id} into equipment slot {index}");
                return;
            }

            // First try to stack into existing slots
            foreach (var slot in GetSlots(index).Where(slot => item.CanAddTo(slot.Item)))
            {
                item.AddTo(slot.Item);
                if (item.Amount <= 0) break;
            }

            // Then put into empty slot
            var emptySlot = GetSlots(index).FirstOrDefault(slot => slot.Item == null);
            if (emptySlot != null)
            {
                emptySlot.Item = new ItemStack(item.Definition, item.Instance, item.Amount);
                item.Amount = 0;
            }
        }

        public void Put(ItemStack item)
        {
            Put(item.GetEquipmentSlotType(), item);
        }

        public void Take(EquipmentSlotType index, ref int amount)
        {
            // Remove from slots until amount is satisfied
            foreach (var slot in GetSlots(index).Where(slot => slot.Item != null))
            {
                if (amount <= 0) break;
                slot.Item.RemoveFrom(ref amount);
                // Clean up empty slots
                if (slot.Item.Amount <= 0) slot.Item = null;
            }
        }

        public void Take(string id, ref int amount)
        {
            // Remove from slots until amount is satisfied
            foreach (var slot in _slots.Where(slot => slot.Item.HasId(id)))
            {
                if (amount <= 0) break;
                slot.Item.RemoveFrom(ref amount);
                // Clean up empty slots
                if (slot.Item.Amount <= 0) slot.Item = null;
            }
        }

        private IEnumerable<EquipmentSlot> GetSlots(EquipmentSlotType slotType)
        {
            return _slots.Where(s => s.SlotType == slotType);
        }
    }
}
