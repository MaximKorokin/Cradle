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
            _firstInventoryViewController = new InventoryViewController(_window.FirstInventoryView);
            _firstInventoryViewController.Bind(_inventoryHudData);
            _secondInventoryViewController = new InventoryViewController(_window.SecondInventoryView);
            _secondInventoryViewController.Bind(_storageHudData);

            _firstInventoryViewController.SlotClick += _previewProcessor.OnFirstItemContainerSlotClick;
            _secondInventoryViewController.SlotClick += _previewProcessor.OnSecondItemContainerSlotClick;

            Redraw();
        }

        public override void Unbind()
        {
            _firstInventoryViewController.SlotClick -= _previewProcessor.OnFirstItemContainerSlotClick;
            _secondInventoryViewController.SlotClick -= _previewProcessor.OnSecondItemContainerSlotClick;

            _firstInventoryViewController.Unbind();
            _secondInventoryViewController.Unbind();

            _window = null;
        }

        private void Redraw()
        {
            _firstInventoryViewController.Redraw();
            _secondInventoryViewController.Redraw();
        }
    }
}
