using Assets._Game.Scripts.Items.Equipment;

namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct UnequipToContainerCommand : IItemCommand
    {
        public readonly IItemContainer ToContainer;
        public readonly EquipmentModel EquipmentModel;
        public readonly EquipmentSlotKey EquipmentSlot;

        public UnequipToContainerCommand(IItemContainer toContainer, EquipmentModel equipmentModel, EquipmentSlotKey equipmentSlot)
        {
            ToContainer = toContainer;
            EquipmentModel = equipmentModel;
            EquipmentSlot = equipmentSlot;
        }
    }
}
