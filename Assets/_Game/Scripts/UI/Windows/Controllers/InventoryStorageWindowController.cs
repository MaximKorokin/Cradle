using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Services;
using Assets._Game.Scripts.UI.Views;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public class InventoryStorageWindowController : WindowControllerBase<InventoryStorageWindow, InventoryStorageWindowControllerArguments>
    {
        private readonly InventoryViewController _firstInventoryViewController;
        private readonly InventoryViewController _secondInventoryViewController;
        private readonly InventoryHudData _inventoryHudData;
        private readonly StorageHudData _storageHudData;
        private readonly EquipmentHudData _equipmentHudData;
        private readonly ItemPreviewService _itemPreviewService;

        public InventoryStorageWindowController(
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

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _storageHudData.SetEntityId(Arguments.StorageEntityId);
            _inventoryHudData.SetEntityId(Arguments.InventoryEntityId);
            _equipmentHudData.SetEntityId(Arguments.EquipmentEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            _firstInventoryViewController.Initialize(Window.FirstInventoryView);
            _firstInventoryViewController.Bind(_inventoryHudData);
            _secondInventoryViewController.Initialize(Window.SecondInventoryView);
            _secondInventoryViewController.Bind(_storageHudData);

            _firstInventoryViewController.SlotClick += OnFirstInventorySlotClick;
            _secondInventoryViewController.SlotClick += OnSecondInventorySlotClick;
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            _firstInventoryViewController.SlotClick -= OnFirstInventorySlotClick;
            _secondInventoryViewController.SlotClick -= OnSecondInventorySlotClick;

            _firstInventoryViewController.Unbind();
            _secondInventoryViewController.Unbind();
        }

        private void OnFirstInventorySlotClick(InventorySlot slot)
        {
            var item = _inventoryHudData.InventoryModel.Get(slot);
            if (item == null) return;

            var equipmentSlotToCompare = _equipmentHudData.EquipmentModel.FindOccupiedSlotForItem(item.Value);

            _itemPreviewService.ShowItemStackPreview(
                slot.ToInt64(),
                ItemContainerPath.Inventory(Arguments.InventoryEntityId.Value),
                ItemContainerPath.Storage(Arguments.StorageEntityId.Value),
                ItemContainerPath.Equipment(Arguments.InventoryEntityId.Value),
                equipmentSlotToCompare);
        }

        private void OnSecondInventorySlotClick(InventorySlot slot)
        {
            var item = _storageHudData.InventoryModel.Get(slot);
            if (item == null) return;

            var equipmentSlotToCompare = _equipmentHudData.EquipmentModel.FindOccupiedSlotForItem(item.Value);

            _itemPreviewService.ShowItemStackPreview(
                slot.ToInt64(),
                ItemContainerPath.Storage(Arguments.StorageEntityId.Value),
                ItemContainerPath.Inventory(Arguments.InventoryEntityId.Value),
                ItemContainerPath.Equipment(Arguments.InventoryEntityId.Value),
                equipmentSlotToCompare);
        }

        protected override void Redraw()
        {
            _firstInventoryViewController.Redraw();
            _secondInventoryViewController.Redraw();
        }
    }

    public readonly struct InventoryStorageWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> StorageEntityId { get; }
        public IReadOnlyObservableData<string> InventoryEntityId { get; }
        public IReadOnlyObservableData<string> EquipmentEntityId { get; }

        public InventoryStorageWindowControllerArguments(
            IReadOnlyObservableData<string> storageEntityId,
            IReadOnlyObservableData<string> inventoryEntityId,
            IReadOnlyObservableData<string> equipmentEntityId)
        {
            InventoryEntityId = inventoryEntityId;
            StorageEntityId = storageEntityId;
            EquipmentEntityId = equipmentEntityId;
        }
    }
}
