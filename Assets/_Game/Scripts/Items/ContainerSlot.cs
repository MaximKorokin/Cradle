using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;

namespace Assets._Game.Scripts.Items
{
    public interface IContainerSlot
    {
        long ToInt64();
    }

    public static class ContainerSlotConverter
    {
        public static long ToInt64<T>(T slot) where T : notnull, IContainerSlot
        {
            return slot.ToInt64();
        }

        public static InventorySlot ToInventorySlot(long value) => InventorySlot.FromInt64(value);

        public static EquipmentSlotKey ToEquipmentSlot(long value) => EquipmentSlotKey.FromInt64(value);
    }
}
