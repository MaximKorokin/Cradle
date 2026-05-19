using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Services;
using Assets._Game.Scripts.UI.Views;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public class InventoryEquipmentWindowController : WindowControllerBase<InventoryEquipmentWindow, InventoryEquipmentWindowControllerArguments>
    {
        private InventoryEquipmentWindow _window;

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

        public override void Initialize(InventoryEquipmentWindowControllerArguments arguments)
        {
            base.Initialize(arguments);

            _inventoryHudData.SetInventoryEntity(Arguments.InventoryEntityId);
            _equipmentHudData.SetEquipmentEntity(Arguments.EquipmentEntityId);
        }

        public override void Bind(InventoryEquipmentWindow window)
        {
            _window = window;
            _inventoryViewController.Initialize(_window.InventoryView);
            _inventoryViewController.Bind(_inventoryHudData);
            _equipmentViewController.Initialize(_window.EquipmentView);
            _equipmentViewController.Bind(_equipmentHudData);

            _inventoryViewController.SlotClick += OnInventorySlotClick;
            _equipmentViewController.SlotClick += OnEquipmentSlotClick;

            Redraw();
        }

        public override void Unbind()
        {
            _inventoryViewController.SlotClick -= OnInventorySlotClick;
            _equipmentViewController.SlotClick -= OnEquipmentSlotClick;

            _inventoryViewController.Unbind();
            _equipmentViewController.Unbind();

            _window = null;
        }

        private void OnInventorySlotClick(InventorySlot slot)
        {
            var item = _inventoryHudData.InventoryModel.Get(slot);
            if (item == null) return;

            var equipmentSlotToCompare = _equipmentHudData.EquipmentModel.FindOccupiedSlotForItem(item.Value);

            _itemPreviewService.ShowItemStackPreview(
                slot.ToInt64(),
                ItemContainerPath.Inventory(Arguments.InventoryEntityId),
                ItemContainerPath.Equipment(Arguments.EquipmentEntityId),
                ItemContainerPath.Equipment(Arguments.EquipmentEntityId),
                equipmentSlotToCompare);
        }

        private void OnEquipmentSlotClick(EquipmentSlotKey slot)
        {
            var item = _equipmentHudData.EquipmentModel.Get(slot);
            if (item == null) return;

            _itemPreviewService.ShowItemStackPreview(
                slot.ToInt64(),
                ItemContainerPath.Equipment(Arguments.EquipmentEntityId),
                ItemContainerPath.Inventory(Arguments.InventoryEntityId),
                ItemContainerPath.Equipment(Arguments.EquipmentEntityId),
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
        public string InventoryEntityId { get; }
        public string EquipmentEntityId { get; }

        public InventoryEquipmentWindowControllerArguments(string inventoryEntityId, string equipmentEntityId)
        {
            InventoryEntityId = inventoryEntityId;
            EquipmentEntityId = equipmentEntityId;
        }
    }
}
