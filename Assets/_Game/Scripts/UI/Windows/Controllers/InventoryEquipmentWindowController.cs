using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Windows.Shared;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public class InventoryEquipmentWindowController : WindowControllerBase<InventoryEquipmentWindow, EmptyWindowControllerArguments>
    {
        private InventoryEquipmentWindow _window;
        private readonly IInventoryHudData _inventoryHudData;
        private readonly IEquipmentHudData _equipmentHudData;

        private readonly ItemStacksPreviewInputProcessor<InventorySlot, EquipmentSlotKey> _previewProcessor;

        private InventoryViewController _inventoryViewController;
        private EquipmentViewController _equipmentViewController;

        public InventoryEquipmentWindowController(
            InventoryHudData inventoryHudData,
            EquipmentHudData equipmentHudData,
            WindowManager windowManager)
        {
            _inventoryHudData = inventoryHudData;
            _equipmentHudData = equipmentHudData;

            _previewProcessor = new(windowManager, _equipmentHudData.EquipmentModel, _inventoryHudData.InventoryModel, _equipmentHudData.EquipmentModel, ItemContainerId.Inventory, ItemContainerId.Equipment);
        }

        public override void Bind(InventoryEquipmentWindow window)
        {
            _window = window;
            _inventoryViewController = new InventoryViewController(_window.InventoryView, _inventoryHudData.InventoryModel);
            _equipmentViewController = new EquipmentViewController(_window.EquipmentView);

            _inventoryViewController.SlotClick += _previewProcessor.OnFirstItemContainerSlotClick;
            _equipmentViewController.SlotClick += _previewProcessor.OnSecondItemContainerSlotClick;

            Redraw();
        }

        public override void Dispose()
        {
            _inventoryViewController.SlotClick -= _previewProcessor.OnFirstItemContainerSlotClick;
            _equipmentViewController.SlotClick -= _previewProcessor.OnSecondItemContainerSlotClick;
        }

        private void Redraw()
        {
            _window.Render(_inventoryHudData, _equipmentHudData);
        }
    }
}
