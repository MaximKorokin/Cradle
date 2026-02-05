using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Shared.Extensions;
using System.Linq;

namespace Assets._Game.Scripts.UI.Windows.Shared
{
    public sealed class ItemStacksPreviewInputProcessor<T1, T2>
    {
        private readonly WindowManager _windowManager;
        private readonly EquipmentModel _equipmentModel;
        private readonly IItemContainer<T1> _firtsItemContainer;
        private readonly IItemContainer<T2> _secondItemContainer;
        private readonly ItemCommandHandler _handler;

        private T1 _firstPointerDownSlotIndex;
        private T2 _secondPointerDownSlotIndex;

        public ItemStacksPreviewInputProcessor(
            WindowManager windowManager,
            EquipmentModel equipmentModel,
            IItemContainer<T1> primaryItemContainer,
            IItemContainer<T2> secondaryItemContainer,
            ItemCommandHandler handler)
        {
            _windowManager = windowManager;
            _equipmentModel = equipmentModel;
            _firtsItemContainer = primaryItemContainer;
            _secondItemContainer = secondaryItemContainer;
            _handler = handler;
        }

        public void OnFirstItemContainerSlotPointerDown(T1 slotIndex)
        {
            _firstPointerDownSlotIndex = slotIndex;
        }

        public void OnFirstItemContainerSlotPointerUp(T1 slotIndex)
        {
            if (!_firstPointerDownSlotIndex.Equals(slotIndex)) return;
            var primaryItem = _firtsItemContainer.Get(slotIndex);
            if (primaryItem == null) return;

            _windowManager.ShowItemStackPreviewWindow(
                _equipmentModel,
                GetEquipmentSlotToCompare(primaryItem.Value, _firtsItemContainer),
                _firstPointerDownSlotIndex,
                _firtsItemContainer,
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
                GetEquipmentSlotToCompare(primaryItem.Value, _secondItemContainer),
                _secondPointerDownSlotIndex,
                _secondItemContainer,
                _firtsItemContainer,
                _handler);
        }

        private EquipmentSlotKey? GetEquipmentSlotToCompare(ItemStackSnapshot primaryItem, IItemContainer containerToExcept)
        {
            // The item is equipped, no need to compare
            if ((containerToExcept is EquipmentModel equipmentModel) && equipmentModel == _equipmentModel) return null;

            var equipmentSlotType = primaryItem.GetEquipmentSlotType();
            if (equipmentSlotType == EquipmentSlotType.None)
                return null;
            var itemSlotToCompare = _equipmentModel.Enumerate().FirstOrDefault(x => x.Slot.SlotType == equipmentSlotType && x.Snapshot != null).Slot;
            return itemSlotToCompare;
        }
    }
}
