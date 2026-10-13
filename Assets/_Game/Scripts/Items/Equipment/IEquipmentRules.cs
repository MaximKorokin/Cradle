using Assets._Game.Scripts.Shared.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Items.Equipment
{
    public interface IEquipmentRules
    {
        bool CanEquip(ItemStackSnapshot snapshot);
        bool CanPlace(EquipmentSlotKey slot, ItemStackSnapshot item, EquipmentModel equipmentModel);
        IEnumerable<EquipmentSlotKey> GetConflictingSlots(EquipmentSlotKey targetSlot, ItemStackSnapshot item, EquipmentModel equipmentModel);
    }

    public sealed class DefaultEquipmentRules : IEquipmentRules
    {
        public bool CanEquip(ItemStackSnapshot item)
        {
            var slotType = item.GetEquipmentSlotType();

            return slotType != EquipmentSlotType.None;
        }

        public bool CanPlace(EquipmentSlotKey slot, ItemStackSnapshot item, EquipmentModel equipmentModel)
        {
            var slotType = item.GetEquipmentSlotType();

            if (slotType == EquipmentSlotType.None) return false;

            if (slotType != slot.SlotType) return false;

            return !GetConflictingSlots(slot, item, equipmentModel).Any(conflictingSlot => !conflictingSlot.Equals(slot));
        }

        public IEnumerable<EquipmentSlotKey> GetConflictingSlots(EquipmentSlotKey targetSlot, ItemStackSnapshot item, EquipmentModel equipmentModel)
        {
            var slotsToCheck = new HashSet<EquipmentSlotKey> { targetSlot };
            foreach (var secondarySlot in equipmentModel.GetSecondarySlots(item))
                slotsToCheck.Add(secondarySlot);

            var conflictingSlots = new HashSet<EquipmentSlotKey>();

            // Check if any of the slots to check are already occupied
            foreach (var slotToCheck in slotsToCheck)
            {
                if (equipmentModel.Get(slotToCheck) != null)
                    conflictingSlots.Add(slotToCheck);
            }

            // Check if any equipped item's secondary slots conflict with the slots to check
            foreach (var (blockedSlot, _, blockingSlot) in equipmentModel.EnumerateBlockedSlots())
            {
                if (slotsToCheck.Contains(blockedSlot))
                    conflictingSlots.Add(blockingSlot);
            }

            return conflictingSlots;
        }
    }
}
