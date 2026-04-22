using Assets._Game.Scripts.Infrastructure.Game;
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

        private readonly InventoryViewController _firstInventoryViewController;
        private readonly InventoryViewController _secondInventoryViewController;
        private readonly InventoryHudData _inventoryHudData;
        private readonly StorageHudData _storageHudData;

        private readonly ItemStacksPreviewInputProcessor<InventorySlot, InventorySlot> _previewProcessor;

        public InventoryInventoryWindowController(
            InventoryViewController firstInventoryViewController,
            InventoryViewController secondInventoryViewController,
            InventoryHudData inventoryHudData,
            StorageHudData storageHudData,
            EquipmentHudData equipmentHudData,
            WindowManager windowManager)
        {
            _firstInventoryViewController = firstInventoryViewController;
            _secondInventoryViewController = secondInventoryViewController;
            _inventoryHudData = inventoryHudData;
            _storageHudData = storageHudData;

            _previewProcessor = new(windowManager, equipmentHudData.EquipmentModel, inventoryHudData.InventoryModel, storageHudData.InventoryModel, ItemContainerId.Inventory, ItemContainerId.Storage);
        }

        public override void Bind(InventoryInventoryWindow window)
        {
            _window = window;
            _firstInventoryViewController.Initialize(_window.FirstInventoryView);
            _firstInventoryViewController.Bind(_inventoryHudData);
            _secondInventoryViewController.Initialize(_window.SecondInventoryView);
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
