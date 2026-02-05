using Assets._Game.Scripts.Items.Equipment;

namespace Assets._Game.Scripts.Items.Commands
{
    public class EquipFromContainerCommand : IItemCommand
    {
        public IItemContainer FromContainer;
        public ItemStack ItemStack;
        public EquipmentModel EquipmentModel;
    }
}
