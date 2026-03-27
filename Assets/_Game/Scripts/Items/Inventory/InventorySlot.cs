namespace Assets._Game.Scripts.Items.Inventory
{
    public readonly struct InventorySlot : IContainerSlot
    {
        public readonly int Index;

        public InventorySlot(int index)
        {
            Index = index;
        }

        public long ToInt64() => Index;

        public static InventorySlot FromInt64(long value) => new((int)value);

        public static implicit operator InventorySlot(int index) => new(index);
        public static implicit operator int(InventorySlot slot) => slot.Index;
    }
}
