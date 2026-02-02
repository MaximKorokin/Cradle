namespace Assets._Game.Scripts.Items.Equipment
{
    public class EquipmentSlot
    {
        public EquipmentSlotType SlotType;
        public ItemStack Item;

        public EquipmentSlot(EquipmentSlotType slotType)
        {
            SlotType = slotType;
            Item = null;
        }
    }

    public enum EquipmentSlotType
    {
        None = 0,
        Weapon = 10,
        Helmet = 20,
        Armor = 30,
        Gloves = 40,
        Boots = 50,
        Ring = 60,
        Consumable = 1000,
    }
}
