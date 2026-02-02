using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using System;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryEquipmentWindowController : IDisposable
    {
        private readonly InventoryEquipmentWindow _window;
        private readonly ItemCommandHandler _handler;
        private readonly InventoryModel _inventoryModel;
        private readonly EquipmentModel _equipmentModel;

        public InventoryEquipmentWindowController(
            InventoryEquipmentWindow window,
            InventoryModel inventoryModel,
            EquipmentModel equipmentModel,
            ItemCommandHandler handler)
        {
            _window = window;
            _inventoryModel = inventoryModel;
            _equipmentModel = equipmentModel;
            _handler = handler;

            _inventoryModel.Changed += Redraw;
            _equipmentModel.Changed += Redraw;

            Redraw();
        }

        private void Redraw()
        {
            _window.Render(_inventoryModel, _equipmentModel);
        }

        private void OnEquipClicked(int inventorySlot)
        {
            _handler.Handle(new EquipFromInventoryCommand()
            {
                EquipmentModel = _equipmentModel,
                InventoryModel = _inventoryModel,
                InventorySlot = inventorySlot
            });
        }

        private void OnUnequipClicked(EquipmentSlotType slotType)
        {
            _handler.Handle(new UnequipToInventoryCommand()
            {
                EquipmentModel = _equipmentModel,
                InventoryModel = _inventoryModel,
                SlotType = slotType
            });
        }

        public void Dispose()
        {
            _inventoryModel.Changed -= Redraw;
            _equipmentModel.Changed -= Redraw;
        }
    }
}
