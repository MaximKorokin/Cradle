using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Services;
using Assets._Game.Scripts.UI.Views;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public class StorageWindowController : WindowControllerBase<StorageWindow, StorageWindowControllerArguments>
    {
        private readonly InventoryViewController _stoargeInventoryViewController;
        private readonly InventoryHudData _inventoryHudData;
        private readonly StorageHudData _storageHudData;
        private readonly EquipmentHudData _equipmentHudData;
        private readonly ItemPreviewService _itemPreviewService;

        public StorageWindowController(
            InventoryViewController stoargeInventoryViewController,
            InventoryHudData inventoryHudData,
            StorageHudData storageHudData,
            EquipmentHudData equipmentHudData,
            ItemPreviewService itemPreviewService)
        {
            _stoargeInventoryViewController = stoargeInventoryViewController;
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

            _stoargeInventoryViewController.Initialize(Window.StorageInventoryView);
            _stoargeInventoryViewController.Bind(_storageHudData);

            _stoargeInventoryViewController.SlotClick += OnStoargeInventorySlotClick;
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            _stoargeInventoryViewController.SlotClick -= OnStoargeInventorySlotClick;

            _stoargeInventoryViewController.Unbind();
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

        private void OnStoargeInventorySlotClick(InventorySlot slot)
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
            _stoargeInventoryViewController.Redraw();
        }
    }

    public readonly struct StorageWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> StorageEntityId { get; }
        public IReadOnlyObservableData<string> InventoryEntityId { get; }
        public IReadOnlyObservableData<string> EquipmentEntityId { get; }

        public StorageWindowControllerArguments(
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
