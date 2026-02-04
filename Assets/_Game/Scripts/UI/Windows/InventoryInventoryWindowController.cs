using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.Windows.Shared;
using System;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryInventoryWindowController : IDisposable
    {
        private readonly WindowManager _windowManager;
        private readonly InventoryInventoryWindow _window;
        private readonly InventoryModel _firstInventoryModel;
        private readonly InventoryModel _secondInventoryModel;

        private readonly ItemStacksPreviewInputProcessor<int, int> _previewProcessor;

        public InventoryInventoryWindowController(
            WindowManager windowManager,
            InventoryInventoryWindow window,
            EquipmentModel equipmentModel,
            InventoryModel firstInventoryModel,
            InventoryModel secondInventoryModel,
            ItemCommandHandler handler)
        {
            _windowManager = windowManager;
            _window = window;
            _firstInventoryModel = firstInventoryModel;
            _secondInventoryModel = secondInventoryModel;

            _firstInventoryModel.Changed += Redraw;
            _secondInventoryModel.Changed += Redraw;

            _previewProcessor = new(windowManager, equipmentModel, firstInventoryModel, secondInventoryModel, handler);

            _window.FirstInventorySlotPointerDown += _previewProcessor.OnFirstItemContainerSlotPointerDown;
            _window.FirstInventorySlotPointerUp += _previewProcessor.OnFirstItemContainerSlotPointerUp;
            _window.SecondInventorySlotPointerDown += _previewProcessor.OnSecondItemContainerSlotPointerDown;
            _window.SecondInventorySlotPointerUp += _previewProcessor.OnSecondItemContainerSlotPointerUp;

            Redraw();
        }

        private void Redraw()
        {
            _window.Render(_firstInventoryModel, _secondInventoryModel);
        }

        private void OnCloseClicked()
        {
            _windowManager.CloseTop();
        }

        public void Dispose()
        {
            _firstInventoryModel.Changed -= Redraw;
            _secondInventoryModel.Changed -= Redraw;
        }
    }
}
