namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct EquipFromContainerCommand : IItemCommand
    {
        public readonly ItemContainerId FromContainer;
        public readonly long FromSlot;
        public readonly long EquipmentSlot;

        public EquipFromContainerCommand(ItemContainerId fromContainer, long fromSlot, long equipmentSlot)
        {
            FromContainer = fromContainer;
            FromSlot = fromSlot;
            EquipmentSlot = equipmentSlot;
        }
    }
}
