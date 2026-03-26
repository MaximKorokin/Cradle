using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Windows.Shared;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public class InventoryInventoryWindowController : WindowControllerBase<InventoryInventoryWindow, EmptyWindowControllerArguments>
    {
        private InventoryInventoryWindow _window;
        private readonly InventoryHudData _inventoryHudData;
        private readonly StorageHudData _storageHudData;

        private readonly ItemStacksPreviewInputProcessor<int, int> _previewProcessor;

        public InventoryInventoryWindowController(
            InventoryHudData inventoryHudData,
            StorageHudData storageHudData,
            EquipmentHudData equipmentHudData,
            WindowManager windowManager)
        {
            _inventoryHudData = inventoryHudData;
            _storageHudData = storageHudData;

            _previewProcessor = new(windowManager, equipmentHudData.EquipmentModel, inventoryHudData.InventoryModel, storageHudData.InventoryModel);
        }

        public override void Bind(InventoryInventoryWindow window)
        {
            _window = window;

            _window.FirstInventorySlotPointerDown += _previewProcessor.OnFirstItemContainerSlotPointerDown;
            _window.FirstInventorySlotPointerUp += _previewProcessor.OnFirstItemContainerSlotPointerUp;
            _window.SecondInventorySlotPointerDown += _previewProcessor.OnSecondItemContainerSlotPointerDown;
            _window.SecondInventorySlotPointerUp += _previewProcessor.OnSecondItemContainerSlotPointerUp;

            Redraw();
        }

        public override void Dispose()
        {
            _window.FirstInventorySlotPointerDown -= _previewProcessor.OnFirstItemContainerSlotPointerDown;
            _window.FirstInventorySlotPointerUp -= _previewProcessor.OnFirstItemContainerSlotPointerUp;
            _window.SecondInventorySlotPointerDown -= _previewProcessor.OnSecondItemContainerSlotPointerDown;
            _window.SecondInventorySlotPointerUp -= _previewProcessor.OnSecondItemContainerSlotPointerUp;
        }

        private void Redraw()
        {
            _window.Render(_inventoryHudData, _storageHudData);
        }
    }
}
