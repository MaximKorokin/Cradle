using Assets._Game.Scripts.Items.Equipment;

namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct EquipFromContainerCommand<T> : IItemCommand
    {
        public readonly IItemContainer<T> FromContainer;
        public readonly T FromSlot;
        public readonly EquipmentModel EquipmentModel;
        public readonly EquipmentSlotKey EquipmentSlot;

        public EquipFromContainerCommand(IItemContainer<T> fromContainer, T fromSlot, EquipmentModel equipmentModel, EquipmentSlotKey equipmentSlot)
        {
            FromContainer = fromContainer;
            FromSlot = fromSlot;
            EquipmentModel = equipmentModel;
            EquipmentSlot = equipmentSlot;
        }
    }
}
