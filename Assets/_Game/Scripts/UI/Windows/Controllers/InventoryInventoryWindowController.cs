using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Windows.Shared;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public class InventoryInventoryWindowController : WindowControllerBase<InventoryInventoryWindow, EmptyWindowControllerArguments>
    {
        private InventoryInventoryWindow _window;
        private readonly InventoryHudData _inventoryStashData;
        private readonly StashHudData _stashHudData;

        private readonly ItemStacksPreviewInputProcessor<int, int> _previewProcessor;

        public InventoryInventoryWindowController(
            InventoryHudData inventoryHudData,
            StashHudData stashHudData,
            EquipmentHudData equipmentHudData,
            WindowManager windowManager,
            ItemCommandHandler handler)
        {
            _inventoryStashData = inventoryHudData;
            _stashHudData = stashHudData;

            _previewProcessor = new(windowManager, equipmentHudData.EquipmentModel, inventoryHudData.InventoryModel, stashHudData.InventoryModel, handler);
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
            _window.Render(_inventoryStashData, _stashHudData);
        }
    }
}
