namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct UnequipToContainerCommand : IItemCommand
    {
        public readonly ItemContainerPath ToContainer;
        public readonly ItemContainerPath FromEquipment;
        public readonly long EquipmentSlot;

        public UnequipToContainerCommand(ItemContainerPath toContainer, ItemContainerPath fromEquipment, long equipmentSlot)
        {
            ToContainer = toContainer;
            FromEquipment = fromEquipment;
            EquipmentSlot = equipmentSlot;
        }
    }
}
