namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct EquipFromContainerCommand : IItemCommand
    {
        public readonly ItemContainerPath FromContainer;
        public readonly long FromSlot;
        public readonly ItemContainerPath Equipment;
        public readonly long EquipmentSlot;

        public EquipFromContainerCommand(ItemContainerPath fromContainer, long fromSlot, ItemContainerPath equipment, long equipmentSlot)
        {
            FromContainer = fromContainer;
            FromSlot = fromSlot;
            Equipment = equipment;
            EquipmentSlot = equipmentSlot;
        }
    }
}
