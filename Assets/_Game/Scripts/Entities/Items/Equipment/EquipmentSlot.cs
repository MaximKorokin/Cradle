namespace Assets._Game.Scripts.Entities.Items.Equipment
{
    public class EquipmentSlot
    {
        public EquipmentSlotType SlotType;
        public ItemStack Item;
    }

    public enum EquipmentSlotType
    {
        Weapon,
        Head,
        Body,
        Ring,
    }
}