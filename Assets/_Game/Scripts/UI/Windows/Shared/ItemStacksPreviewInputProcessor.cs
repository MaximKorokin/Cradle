using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.Services;
using System.Linq;

namespace Assets._Game.Scripts.UI.Windows.Shared
{
    public sealed class ItemStacksPreviewInputProcessor<T1, T2>
        where T1 : struct, IContainerSlot
        where T2 : struct, IContainerSlot
    {
        private readonly ItemPreviewService _itemPreviewService;
        private readonly EquipmentModel _equipmentModel;
        private readonly IItemContainer<T1> _firstItemContainer;
        private readonly IItemContainer<T2> _secondItemContainer;
        private readonly ItemContainerId _firstContainerId;
        private readonly ItemContainerId _secondContainerId;

        public ItemStacksPreviewInputProcessor(
            ItemPreviewService itemPreviewService,
            EquipmentModel equipmentModel,
            IItemContainer<T1> primaryItemContainer,
            IItemContainer<T2> secondaryItemContainer,
            ItemContainerId primaryContainerId,
            ItemContainerId secondaryContainerId)
        {
            _itemPreviewService = itemPreviewService;
            _equipmentModel = equipmentModel;
            _firstItemContainer = primaryItemContainer;
            _secondItemContainer = secondaryItemContainer;
            _firstContainerId = primaryContainerId;
            _secondContainerId = secondaryContainerId;
        }

        public void OnFirstItemContainerSlotClick(T1 slotIndex)
        {
            var primaryItem = _firstItemContainer.Get(slotIndex);
            if (primaryItem == null) return;

            _itemPreviewService.ShowItemStackPreview(
                slotIndex.ToInt64(),
                _firstContainerId,
                _secondContainerId,
                GetEquipmentSlotToCompare(primaryItem.Value, _firstItemContainer));
        }

        public void OnSecondItemContainerSlotClick(T2 slotIndex)
        {
            var primaryItem = _secondItemContainer.Get(slotIndex);
            if (primaryItem == null) return;

            _itemPreviewService.ShowItemStackPreview(
                slotIndex.ToInt64(),
                _secondContainerId,
                _firstContainerId,
                GetEquipmentSlotToCompare(primaryItem.Value, _secondItemContainer));
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
