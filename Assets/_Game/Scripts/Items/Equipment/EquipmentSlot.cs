using Assets._Game.Scripts.Items.Inventory;
using System;

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

    public struct EquipmentSlotKey : IContainerSlot
    {
        public EquipmentSlotType SlotType;
        public int Index;

        public EquipmentSlotKey(EquipmentSlotType slotType, int index)
        {
            SlotType = slotType;
            Index = index;
        }

        public readonly long ToInt64()
            => ((long)(int)SlotType << 32) | (uint)Index;

        public static EquipmentSlotKey FromInt64(long value)
            => new((EquipmentSlotType)(int)(value >> 32), (int)(value & 0xFFFFFFFF));

        public readonly bool Equals(EquipmentSlotKey other)
            => SlotType == other.SlotType && Index == other.Index;

        public readonly override bool Equals(object obj)
            => obj is EquipmentSlotKey other && Equals(other);

        public readonly override int GetHashCode()
            => HashCode.Combine((int)SlotType, Index);

        public readonly override string ToString()
            => $"{SlotType}:{Index}";
    }
}
