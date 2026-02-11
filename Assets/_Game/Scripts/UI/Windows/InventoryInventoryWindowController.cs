using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.Windows.Shared;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryInventoryWindowController : WindowControllerBase<InventoryInventoryWindow, EmptyWindowControllerArguments>
    {
        private InventoryInventoryWindow _window;
        private readonly InventoryModel _firstInventoryModel;
        private readonly InventoryModel _secondInventoryModel;

        private readonly ItemStacksPreviewInputProcessor<int, int> _previewProcessor;

        public InventoryInventoryWindowController(
            WindowManager windowManager,
            PlayerContext playerContext,
            ItemCommandHandler handler)
        {
            _firstInventoryModel = playerContext.IEModule.Inventory;
            _secondInventoryModel = playerContext.StashInventory;

            _firstInventoryModel.Changed += Redraw;
            _secondInventoryModel.Changed += Redraw;

            _previewProcessor = new(windowManager, playerContext.IEModule.Equipment, _firstInventoryModel, _secondInventoryModel, handler);
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
            _firstInventoryModel.Changed -= Redraw;
            _secondInventoryModel.Changed -= Redraw;

            _window.FirstInventorySlotPointerDown -= _previewProcessor.OnFirstItemContainerSlotPointerDown;
            _window.FirstInventorySlotPointerUp -= _previewProcessor.OnFirstItemContainerSlotPointerUp;
            _window.SecondInventorySlotPointerDown -= _previewProcessor.OnSecondItemContainerSlotPointerDown;
            _window.SecondInventorySlotPointerUp -= _previewProcessor.OnSecondItemContainerSlotPointerUp;
        }

        private void Redraw()
        {
            _window.Render(_firstInventoryModel, _secondInventoryModel);
        }
    }
}
