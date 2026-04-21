using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Windows.Shared;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public class InventoryInventoryWindowController : WindowControllerBase<InventoryInventoryWindow, EmptyWindowControllerArguments>
    {
        private InventoryInventoryWindow _window;
        private readonly InventoryHudData _inventoryHudData;
        private readonly StorageHudData _storageHudData;

        private readonly ItemStacksPreviewInputProcessor<InventorySlot, InventorySlot> _previewProcessor;

        private InventoryViewController _firstInventoryViewController;
        private InventoryViewController _secondInventoryViewController;

        public InventoryInventoryWindowController(
            InventoryHudData inventoryHudData,
            StorageHudData storageHudData,
            EquipmentHudData equipmentHudData,
            WindowManager windowManager)
        {
            _inventoryHudData = inventoryHudData;
            _storageHudData = storageHudData;

            _previewProcessor = new(windowManager, equipmentHudData.EquipmentModel, inventoryHudData.InventoryModel, storageHudData.InventoryModel, ItemContainerId.Inventory, ItemContainerId.Storage);
        }

        public override void Bind(InventoryInventoryWindow window)
        {
            _window = window;
            _firstInventoryViewController = new InventoryViewController(_window.FirstInventoryView, _inventoryHudData.InventoryModel);
            _secondInventoryViewController = new InventoryViewController(_window.SecondInventoryView, _storageHudData.InventoryModel);

            _firstInventoryViewController.SlotClick += _previewProcessor.OnFirstItemContainerSlotClick;
            _secondInventoryViewController.SlotClick += _previewProcessor.OnSecondItemContainerSlotClick;

            Redraw();
        }

        public override void Dispose()
        {
            _firstInventoryViewController.SlotClick -= _previewProcessor.OnFirstItemContainerSlotClick;
            _secondInventoryViewController.SlotClick -= _previewProcessor.OnSecondItemContainerSlotClick;
        }

        private void Redraw()
        {
            _window.Render(_inventoryHudData, _storageHudData);
        }
    }
}
