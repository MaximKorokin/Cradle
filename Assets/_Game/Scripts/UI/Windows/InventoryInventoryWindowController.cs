using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Inventory;
using System;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryInventoryWindowController : IDisposable
    {
        private readonly InventoryInventoryWindow _window;
        private readonly ItemCommandHandler _handler;
        private readonly InventoryModel _firstInventoryModel;
        private readonly InventoryModel _secondInventoryModel;

        public InventoryInventoryWindowController(
            InventoryInventoryWindow window,
            InventoryModel firstInventoryModel,
            InventoryModel secondInventoryModel,
            ItemCommandHandler handler)
        {
            _window = window;
            _firstInventoryModel = firstInventoryModel;
            _secondInventoryModel = secondInventoryModel;
            _handler = handler;

            _firstInventoryModel.Changed += Redraw;
            _secondInventoryModel.Changed += Redraw;

            Redraw();
        }

        private void Redraw()
        {
            _window.Render(_firstInventoryModel, _secondInventoryModel);
        }

        private void OnTransferFromFirstClicked(int inventorySlot)
        {
            _handler.Handle(new MoveItemCommand()
            {
                From = _firstInventoryModel,
                FromSlot = inventorySlot,
                To = _secondInventoryModel,
                Amount = int.MaxValue
            });
        }

        private void OnTransferFromSecondClicked(int inventorySlot)
        {
            _handler.Handle(new MoveItemCommand()
            {
                From = _secondInventoryModel,
                FromSlot = inventorySlot,
                To = _firstInventoryModel,
                Amount = int.MaxValue
            });
        }

        public void Dispose()
        {
            _firstInventoryModel.Changed -= Redraw;
            _secondInventoryModel.Changed -= Redraw;
        }
    }
}
