using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using System.Linq;

namespace Assets._Game.Scripts.Items.Equipment
{
    public interface IEquipmentRules
    {
        bool CanEquip(ItemStackSnapshot snapshot);
        bool CanPlace(EquipmentSlotKey slot, ItemStackSnapshot item, EquipmentModel equipmentModel);
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

            // Check if the target slot is occupied by another item's secondary slots
            if (IsSlotOccupiedBySecondarySlots(slot, equipmentModel))
                return false;

            return true;
        }

        private bool IsSlotOccupiedBySecondarySlots(EquipmentSlotKey targetSlot, EquipmentModel equipmentModel)
        {
            // Check all equipped items to see if any of them claim this slot as a secondary slot
            foreach (var (slot, snapshot) in equipmentModel.Enumerate())
            {
                if (snapshot == null) continue;
                if (slot.Equals(targetSlot)) continue; // Skip the target slot itself

                // Check if this item has secondary slots
                if (!snapshot.Value.Definition.TryGetTrait<EquippableTrait>(out var equippableTrait))
                    continue;

                if (equippableTrait.SecondarySlots == null || equippableTrait.SecondarySlots.Length == 0)
                    continue;

                // Check if the target slot is in this item's secondary slots
                if (equippableTrait.SecondarySlots.Contains(targetSlot.SlotType))
                    return true;
            }

            return false;
        }
    }
}
