using Assets._Game.Scripts.Items.Equipment;

namespace Assets._Game.Scripts.Items.Commands
{
    public class UnequipToContainerCommand : IItemCommand
    {
        public IItemContainer ToContainer;
        public EquipmentModel EquipmentModel;
        public EquipmentSlotKey SlotKey;
    }
}
