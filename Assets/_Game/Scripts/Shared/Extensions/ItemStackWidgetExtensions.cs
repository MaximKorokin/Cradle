using Assets._Game.Scripts.UI.Views.Widgets;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class ItemStackWidgetExtensions
    {
        public static long GetItemSlotIndex(this ItemStackWidget itemStackWidget)
        {
            long slotIndex;
            if (itemStackWidget.Type == ItemStackWidgetType.Equipment && itemStackWidget.TryGetComponentInParent<EquipmentSlotWidget>(out var equipmentSlotWidget))
            {
                slotIndex = equipmentSlotWidget.BlockingSlotIndex ?? itemStackWidget.SlotIndex;
            }
            else
            {
                slotIndex = itemStackWidget.SlotIndex;
            }
            return slotIndex;
        }
    }
}
