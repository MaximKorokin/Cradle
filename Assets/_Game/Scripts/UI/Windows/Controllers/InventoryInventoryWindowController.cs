using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Services;
using Assets._Game.Scripts.UI.Views;
using System.Linq;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public class InventoryInventoryWindowController : WindowControllerBase<InventoryInventoryWindow, EmptyWindowControllerArguments>
    {
        private InventoryInventoryWindow _window;

        private readonly InventoryViewController _firstInventoryViewController;
        private readonly InventoryViewController _secondInventoryViewController;
        private readonly InventoryHudData _inventoryHudData;
        private readonly StorageHudData _storageHudData;
        private readonly EquipmentHudData _equipmentHudData;
        private readonly ItemPreviewService _itemPreviewService;

        public InventoryInventoryWindowController(
            InventoryViewController firstInventoryViewController,
            InventoryViewController secondInventoryViewController,
            InventoryHudData inventoryHudData,
            StorageHudData storageHudData,
            EquipmentHudData equipmentHudData,
            ItemPreviewService itemPreviewService)
        {
            _firstInventoryViewController = firstInventoryViewController;
            _secondInventoryViewController = secondInventoryViewController;
            _inventoryHudData = inventoryHudData;
            _storageHudData = storageHudData;
            _equipmentHudData = equipmentHudData;
            _itemPreviewService = itemPreviewService;
        }

        public override void Bind(InventoryInventoryWindow window)
        {
            _window = window;
            _firstInventoryViewController.Initialize(_window.FirstInventoryView);
            _firstInventoryViewController.Bind(_inventoryHudData);
            _secondInventoryViewController.Initialize(_window.SecondInventoryView);
            _secondInventoryViewController.Bind(_storageHudData);

            _firstInventoryViewController.SlotClick += OnFirstInventorySlotClick;
            _secondInventoryViewController.SlotClick += OnSecondInventorySlotClick;

            Redraw();
        }

        public override void Unbind()
        {
            _firstInventoryViewController.SlotClick -= OnFirstInventorySlotClick;
            _secondInventoryViewController.SlotClick -= OnSecondInventorySlotClick;

            _firstInventoryViewController.Unbind();
            _secondInventoryViewController.Unbind();

            _window = null;
        }

        private void OnFirstInventorySlotClick(InventorySlot slot)
        {
            var item = _inventoryHudData.InventoryModel.Get(slot);
            if (item == null) return;

            _itemPreviewService.ShowItemStackPreview(
                slot.ToInt64(),
                ItemContainerId.Inventory,
                ItemContainerId.Storage,
                GetEquipmentSlotToCompare(item.Value));
        }

        private void OnSecondInventorySlotClick(InventorySlot slot)
        {
            var item = _storageHudData.InventoryModel.Get(slot);
            if (item == null) return;

            _itemPreviewService.ShowItemStackPreview(
                slot.ToInt64(),
                ItemContainerId.Storage,
                ItemContainerId.Inventory,
                GetEquipmentSlotToCompare(item.Value));
        }

        private Items.Equipment.EquipmentSlotKey? GetEquipmentSlotToCompare(ItemStackSnapshot item)
        {
            var equipmentSlotType = item.GetEquipmentSlotType();
            if (equipmentSlotType == Items.Equipment.EquipmentSlotType.None)
                return null;

            var itemSlotToCompare = _equipmentHudData.EquipmentModel.Enumerate()
                .FirstOrDefault(x => x.Slot.SlotType == equipmentSlotType && x.Snapshot != null).Slot;
            return itemSlotToCompare;
        }

        private void Redraw()
        {
            _firstInventoryViewController.Redraw();
            _secondInventoryViewController.Redraw();
        }
    }
}
