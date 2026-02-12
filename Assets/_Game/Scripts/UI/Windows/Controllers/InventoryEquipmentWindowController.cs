using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Windows.Shared;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public class InventoryEquipmentWindowController : WindowControllerBase<InventoryEquipmentWindow, EmptyWindowControllerArguments>
    {
        private InventoryEquipmentWindow _window;
        private readonly IInventoryHudData _inventoryHudData;
        private readonly IEquipmentHudData _equipmentHudData;

        private readonly ItemStacksPreviewInputProcessor<int, EquipmentSlotKey> _previewProcessor;

        public InventoryEquipmentWindowController(
            InventoryHudData inventoryHudData,
            EquipmentHudData equipmentHudData,
            WindowManager windowManager,
            ItemCommandHandler handler)
        {
            _inventoryHudData = inventoryHudData;

            _equipmentHudData = equipmentHudData;

            _previewProcessor = new(windowManager, _equipmentHudData.EquipmentModel, inventoryHudData.InventoryModel, _equipmentHudData.EquipmentModel, handler);
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
            _window.InventorySlotPointerDown -= _previewProcessor.OnFirstItemContainerSlotPointerDown;
            _window.InventorySlotPointerUp -= _previewProcessor.OnFirstItemContainerSlotPointerUp;
            _window.EquipmentSlotPointerDown -= _previewProcessor.OnSecondItemContainerSlotPointerDown;
            _window.EquipmentSlotPointerUp -= _previewProcessor.OnSecondItemContainerSlotPointerUp;
        }

        private void Redraw()
        {
            _window.Render(_inventoryHudData, _equipmentHudData);
        }
    }
}
