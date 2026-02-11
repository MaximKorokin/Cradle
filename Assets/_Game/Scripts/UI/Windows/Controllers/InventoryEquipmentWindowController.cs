using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.Windows.Shared;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public class InventoryEquipmentWindowController : WindowControllerBase<InventoryEquipmentWindow, EmptyWindowControllerArguments>
    {
        private InventoryEquipmentWindow _window;
        private readonly InventoryModel _inventoryModel;
        private readonly EquipmentModel _equipmentModel;

        private readonly ItemStacksPreviewInputProcessor<int, EquipmentSlotKey> _previewProcessor;

        public InventoryEquipmentWindowController(
            WindowManager windowManager,
            PlayerContext playerContext,
            ItemCommandHandler handler)
        {
            _inventoryModel = playerContext.IEModule.Inventory;
            _equipmentModel = playerContext.IEModule.Equipment;

            _inventoryModel.Changed += Redraw;
            _equipmentModel.Changed += Redraw;

            _previewProcessor = new(windowManager, _equipmentModel, _inventoryModel, _equipmentModel, handler);
        }

        public override void Bind(InventoryEquipmentWindow window)
        {
            _window = window;

            _window.InventorySlotPointerDown += _previewProcessor.OnFirstItemContainerSlotPointerDown;
            _window.InventorySlotPointerUp += _previewProcessor.OnFirstItemContainerSlotPointerUp;
            _window.EquipmentSlotPointerDown += _previewProcessor.OnSecondItemContainerSlotPointerDown;
            _window.EquipmentSlotPointerUp += _previewProcessor.OnSecondItemContainerSlotPointerUp;

            Redraw();
        }

        public override void Dispose()
        {
            _inventoryModel.Changed -= Redraw;
            _equipmentModel.Changed -= Redraw;

            _window.InventorySlotPointerDown -= _previewProcessor.OnFirstItemContainerSlotPointerDown;
            _window.InventorySlotPointerUp -= _previewProcessor.OnFirstItemContainerSlotPointerUp;
            _window.EquipmentSlotPointerDown -= _previewProcessor.OnSecondItemContainerSlotPointerDown;
            _window.EquipmentSlotPointerUp -= _previewProcessor.OnSecondItemContainerSlotPointerUp;
        }

        private void Redraw()
        {
            _window.Render(_inventoryModel, _equipmentModel);
        }
    }
}
