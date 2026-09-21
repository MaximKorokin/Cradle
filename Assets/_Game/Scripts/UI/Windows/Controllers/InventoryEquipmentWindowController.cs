using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Services;
using Assets._Game.Scripts.UI.Views;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public class InventoryEquipmentWindowController : WindowControllerBase<InventoryEquipmentWindow, InventoryEquipmentWindowControllerArguments>
    {
        private readonly InventoryViewController _inventoryViewController;
        private readonly EquipmentViewController _equipmentViewController;
        private readonly IInventoryHudData _inventoryHudData;
        private readonly IEquipmentHudData _equipmentHudData;
        private readonly ItemPreviewService _itemPreviewService;

        public InventoryEquipmentWindowController(
            InventoryViewController inventoryViewController,
            EquipmentViewController equipmentViewController,
            InventoryHudData inventoryHudData,
            EquipmentHudData equipmentHudData,
            ItemPreviewService itemPreviewService)
        {
            _inventoryViewController = inventoryViewController;
            _equipmentViewController = equipmentViewController;
            _inventoryHudData = inventoryHudData;
            _equipmentHudData = equipmentHudData;
            _itemPreviewService = itemPreviewService;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _inventoryHudData.SetEntityId(Arguments.InventoryEntityId);
            _equipmentHudData.SetEntityId(Arguments.EquipmentEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            _inventoryViewController.Initialize(Window.InventoryView);
            _inventoryViewController.Bind(_inventoryHudData);
            _equipmentViewController.Initialize(Window.EquipmentView);
            _equipmentViewController.Bind(_equipmentHudData);

            _inventoryViewController.SlotClick += OnInventorySlotClick;
            _equipmentViewController.SlotClick += OnEquipmentSlotClick;

            Redraw();
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            _inventoryViewController.SlotClick -= OnInventorySlotClick;
            _equipmentViewController.SlotClick -= OnEquipmentSlotClick;

            _inventoryViewController.Unbind();
            _equipmentViewController.Unbind();
        }

        private void OnInventorySlotClick(InventorySlot slot)
        {
            var item = _inventoryHudData.InventoryModel.Get(slot);
            if (item == null) return;

            var equipmentSlotToCompare = _equipmentHudData.EquipmentModel.FindOccupiedSlotForItem(item.Value);

            _itemPreviewService.ShowItemStackPreview(
                slot.ToInt64(),
                ItemContainerPath.Inventory(Arguments.InventoryEntityId.Value),
                ItemContainerPath.Equipment(Arguments.EquipmentEntityId.Value),
                ItemContainerPath.Equipment(Arguments.EquipmentEntityId.Value),
                equipmentSlotToCompare);
        }

        private void OnEquipmentSlotClick(EquipmentSlotKey slot)
        {
            var item = _equipmentHudData.EquipmentModel.Get(slot);
            if (item == null) return;

            _itemPreviewService.ShowItemStackPreview(
                slot.ToInt64(),
                ItemContainerPath.Equipment(Arguments.EquipmentEntityId.Value),
                ItemContainerPath.Inventory(Arguments.InventoryEntityId.Value),
                ItemContainerPath.Equipment(Arguments.EquipmentEntityId.Value),
                null);
        }

        private void Redraw()
        {
            _inventoryViewController.Redraw();
            _equipmentViewController.Redraw();
        }
    }

    public readonly struct InventoryEquipmentWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> InventoryEntityId { get; }
        public IReadOnlyObservableData<string> EquipmentEntityId { get; }

        public InventoryEquipmentWindowControllerArguments(IReadOnlyObservableData<string> inventoryEntityId, IReadOnlyObservableData<string> equipmentEntityId)
        {
            InventoryEntityId = inventoryEntityId;
            EquipmentEntityId = equipmentEntityId;
        }
    }
}
