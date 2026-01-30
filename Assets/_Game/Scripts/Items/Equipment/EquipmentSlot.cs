namespace Assets._Game.Scripts.Items.Equipment
{
    public class EquipmentSlot
    {
        public EquipmentSlotType SlotType;
        public ItemStack Item;
    }

    public enum EquipmentSlotType
    {
        None = 0,
        Weapon = 10,
        Head = 20,
        Body = 30,
        Gloves = 40,
        Boots = 50,
        Ring = 60,
    }
}
