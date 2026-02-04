using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Shared.Extensions;

namespace Assets._Game.Scripts.UI.Windows.Shared
{
    public sealed class ItemStacksPreviewInputProcessor<T1, T2>
    {
        private readonly WindowManager _windowManager;
        private readonly EquipmentModel _equipmentModel;
        private readonly IItemContainer<T1> _firstItemContainer;
        private readonly IItemContainer<T2> _secondItemContainer;
        private readonly ItemCommandHandler _handler;

        private T1 _firstPointerDownSlotIndex;
        private T2 _secondPointerDownSlotIndex;

        public ItemStacksPreviewInputProcessor(
            WindowManager windowManager,
            EquipmentModel equipmentModel,
            IItemContainer<T1> firstItemContainer,
            IItemContainer<T2> secondItemContainer,
            ItemCommandHandler handler)
        {
            _windowManager = windowManager;
            _equipmentModel = equipmentModel;
            _firstItemContainer = firstItemContainer;
            _secondItemContainer = secondItemContainer;
            _handler = handler;
        }

        public void OnFirstItemContainerSlotPointerDown(T1 slotIndex)
        {
            _firstPointerDownSlotIndex = slotIndex;
        }

        public void OnFirstItemContainerSlotPointerUp(T1 slotIndex)
        {
            if (!_firstPointerDownSlotIndex.Equals(slotIndex)) return;

            var primaryItem = _firstItemContainer.Get(slotIndex);
            if (primaryItem == null) return;

            _windowManager.ShowItemStackPreviewWindow(
                _equipmentModel,
                primaryItem,
                GetItemToCompare(primaryItem),
                _firstItemContainer,
                _secondItemContainer,
                _handler);
        }

        public void OnSecondItemContainerSlotPointerDown(T2 slotIndex)
        {
            _secondPointerDownSlotIndex = slotIndex;
        }

        public void OnSecondItemContainerSlotPointerUp(T2 slotIndex)
        {
            if (!_secondPointerDownSlotIndex.Equals(slotIndex)) return;

            var primaryItem = _secondItemContainer.Get(slotIndex);
            if (primaryItem == null) return;

            _windowManager.ShowItemStackPreviewWindow(
                _equipmentModel,
                primaryItem,
                GetItemToCompare(primaryItem),
                _secondItemContainer,
                _firstItemContainer,
                _handler);
        }

        private ItemStack GetItemToCompare(ItemStack primaryItem)
        {
            var equipmentSlotType = primaryItem.GetEquipmentSlotType();
            if (equipmentSlotType == EquipmentSlotType.None) return null;
            return _equipmentModel.Get(equipmentSlotType);
        }
    }
}
