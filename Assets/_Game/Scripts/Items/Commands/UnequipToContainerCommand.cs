namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct UnequipToContainerCommand : IItemCommand
    {
        public readonly ItemContainerId ToContainer;
        public readonly long EquipmentSlot;

        public UnequipToContainerCommand(ItemContainerId toContainer, long equipmentSlot)
        {
            ToContainer = toContainer;
            EquipmentSlot = equipmentSlot;
        }
    }
}
