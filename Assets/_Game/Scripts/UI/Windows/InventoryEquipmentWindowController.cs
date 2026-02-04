using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.Windows.Shared;
using System;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryEquipmentWindowController : IDisposable
    {
        private readonly WindowManager _windowManager;
        private readonly InventoryEquipmentWindow _window;
        private readonly InventoryModel _inventoryModel;
        private readonly EquipmentModel _equipmentModel;

        private readonly ItemStacksPreviewInputProcessor<int, EquipmentSlotType> _previewProcessor;

        public InventoryEquipmentWindowController(
            WindowManager windowManager,
            InventoryEquipmentWindow window,
            InventoryModel inventoryModel,
            EquipmentModel equipmentModel,
            ItemCommandHandler handler)
        {
            _windowManager = windowManager;
            _window = window;
            _inventoryModel = inventoryModel;
            _equipmentModel = equipmentModel;

            _inventoryModel.Changed += Redraw;
            _equipmentModel.Changed += Redraw;

            _previewProcessor = new(windowManager, equipmentModel, inventoryModel, equipmentModel, handler);

            _window.InventorySlotPointerDown += _previewProcessor.OnFirstItemContainerSlotPointerDown;
            _window.InventorySlotPointerUp += _previewProcessor.OnFirstItemContainerSlotPointerUp;
            _window.EquipmentSlotPointerDown += _previewProcessor.OnSecondItemContainerSlotPointerDown;
            _window.EquipmentSlotPointerUp += _previewProcessor.OnSecondItemContainerSlotPointerUp;

            Redraw();
        }

        private void Redraw()
        {
            _window.Render(_inventoryModel, _equipmentModel);
        }

        public void Dispose()
        {
            _inventoryModel.Changed -= Redraw;
            _equipmentModel.Changed -= Redraw;

            _window.InventorySlotPointerDown -= _previewProcessor.OnFirstItemContainerSlotPointerDown;
            _window.InventorySlotPointerUp -= _previewProcessor.OnFirstItemContainerSlotPointerUp;
            _window.EquipmentSlotPointerDown -= _previewProcessor.OnSecondItemContainerSlotPointerDown;
            _window.EquipmentSlotPointerUp -= _previewProcessor.OnSecondItemContainerSlotPointerUp;
        }

        private void OnCloseClicked()
        {
            _windowManager.CloseTop();
        }
    }
}
