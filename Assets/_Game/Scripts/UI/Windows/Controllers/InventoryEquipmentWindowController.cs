using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Services;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Windows.Shared;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public class InventoryEquipmentWindowController : WindowControllerBase<InventoryEquipmentWindow, EmptyWindowControllerArguments>
    {
        private InventoryEquipmentWindow _window;

        private readonly InventoryViewController _inventoryViewController;
        private readonly EquipmentViewController _equipmentViewController;
        private readonly IInventoryHudData _inventoryHudData;
        private readonly IEquipmentHudData _equipmentHudData;

        private readonly ItemStacksPreviewInputProcessor<InventorySlot, EquipmentSlotKey> _previewProcessor;

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

            _previewProcessor = new(
                itemPreviewService,
                _equipmentHudData.EquipmentModel,
                _inventoryHudData.InventoryModel,
                _equipmentHudData.EquipmentModel,
                ItemContainerId.Inventory,
                ItemContainerId.Equipment);
        }

        public override void Bind(InventoryEquipmentWindow window)
        {
            _window = window;
            _inventoryViewController.Initialize(_window.InventoryView);
            _inventoryViewController.Bind(_inventoryHudData);
            _equipmentViewController.Initialize(_window.EquipmentView);
            _equipmentViewController.Bind(_equipmentHudData);

            _inventoryViewController.SlotClick += _previewProcessor.OnFirstItemContainerSlotClick;
            _equipmentViewController.SlotClick += _previewProcessor.OnSecondItemContainerSlotClick;

            Redraw();
        }

        public override void Unbind()
        {
            _inventoryViewController.SlotClick -= _previewProcessor.OnFirstItemContainerSlotClick;
            _equipmentViewController.SlotClick -= _previewProcessor.OnSecondItemContainerSlotClick;

            _inventoryViewController.Unbind();
            _equipmentViewController.Unbind();

            _window = null;
        }

        private void Redraw()
        {
            _inventoryViewController.Redraw();
            _equipmentViewController.Redraw();
        }
    }
}
