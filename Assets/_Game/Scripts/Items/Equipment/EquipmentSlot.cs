namespace Assets._Game.Scripts.Items.Equipment
{
    public enum EquipmentSlotType
    {
        None = 0,
        Weapon = 10,
        Helmet = 20,
        Armor = 30,
        Gloves = 40,
        Boots = 50,
        Ring = 60,
        Necklace = 70,
        Consumable = 1000,
    }

    public struct EquipmentSlotKey
    {
        public EquipmentSlotType SlotType;
        public int Index;
        public EquipmentSlotKey(EquipmentSlotType slotType, int index)
        {
            SlotType = slotType;
            Index = index;
        }
    }
}
