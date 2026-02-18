using Assets._Game.Scripts.Shared.Extensions;

namespace Assets._Game.Scripts.Items.Equipment
{
    public interface IEquipmentRules
    {
        bool CanEquip(ItemStackSnapshot snapshot);
        bool CanPlace(EquipmentSlotKey slot, ItemStackSnapshot item);
    }

    public sealed class DefaultEquipmentRules : IEquipmentRules
    {
        public bool CanEquip(ItemStackSnapshot item)
        {
            var slotType = item.GetEquipmentSlotType();

            return slotType != EquipmentSlotType.None;
        }

        public bool CanPlace(EquipmentSlotKey slot, ItemStackSnapshot item)
        {
            var slotType = item.GetEquipmentSlotType();

            if (slotType == EquipmentSlotType.None) return false;

            return slotType == slot.SlotType;
        }
    }
}
