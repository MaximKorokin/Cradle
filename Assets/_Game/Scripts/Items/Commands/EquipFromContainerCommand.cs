using Assets._Game.Scripts.Items.Equipment;

namespace Assets._Game.Scripts.Items.Commands
{
    public class EquipFromContainerCommand<T> : IItemCommand
    {
        public IItemContainer<T> FromContainer;
        public T FromSlot;
        public EquipmentModel EquipmentModel;
        public EquipmentSlotKey EquipmentSlot;
    }
}
