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
        private readonly IItemContainer<T1> _firstItemContainer;
        private readonly IItemContainer<T2> _secondItemContainer;

        private T1 _firstPointerDownSlotIndex;
        private T2 _secondPointerDownSlotIndex;

        public ItemStacksPreviewInputProcessor(
            WindowManager windowManager,
            EquipmentModel equipmentModel,
            IItemContainer<T1> primaryItemContainer,
            IItemContainer<T2> secondaryItemContainer)
        {
            _windowManager = windowManager;
            _equipmentModel = equipmentModel;
            _firstItemContainer = primaryItemContainer;
            _secondItemContainer = secondaryItemContainer;
        }

        public void OnFirstItemContainerSlotPointerDown(T1 slotIndex)
        {
            _firstPointerDownSlotIndex = slotIndex;
        }

        public void OnSecondItemContainerSlotPointerDown(T2 slotIndex)
        {
            _secondPointerDownSlotIndex = slotIndex;
        }

        public void OnFirstItemContainerSlotPointerUp(T1 slotIndex)
        {
            if (!_firstPointerDownSlotIndex.Equals(slotIndex)) return;
            var primaryItem = _firstItemContainer.Get(slotIndex);
            if (primaryItem == null) return;

            _windowManager.InstantiateWindow<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments<T1, T2>>(
                new(
                    _equipmentModel,
                    GetEquipmentSlotToCompare(primaryItem.Value, _firstItemContainer),
                    _firstItemContainer,
                    _firstPointerDownSlotIndex,
                    _secondItemContainer),
                typeof(T1),
                typeof(T2));
        }

        public void OnSecondItemContainerSlotPointerUp(T2 slotIndex)
        {
            if (!_secondPointerDownSlotIndex.Equals(slotIndex)) return;
            var primaryItem = _secondItemContainer.Get(slotIndex);
            if (primaryItem == null) return;

            _windowManager.InstantiateWindow<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments<T2, T1>>(
                new(
                    _equipmentModel,
                    GetEquipmentSlotToCompare(primaryItem.Value, _secondItemContainer),
                    _secondItemContainer,
                    _secondPointerDownSlotIndex,
                    _firstItemContainer),
                typeof(T2),
                typeof(T1));
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
