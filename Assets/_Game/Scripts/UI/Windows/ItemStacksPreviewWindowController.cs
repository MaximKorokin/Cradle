using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using System;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class ItemStacksPreviewWindowController : IDisposable
    {
        private readonly ItemStacksPreviewWindow _window;
        private readonly ItemCommandHandler _handler;
        private readonly EquipmentModel _equipmentModel;
        private readonly ItemStack _firstItemStack;
        private readonly ItemStack _secondItemStack;
        private readonly IItemContainer _primaryItemContainer;
        private readonly IItemContainer _secondaryItemContainer;

        public ItemStacksPreviewWindowController(
            WindowManager windowManager,
            ItemStacksPreviewWindow window,
            EquipmentModel equipmentModel,
            ItemStack primaryItemStack,
            ItemStack secondaryItemStack,
            IItemContainer primaryItemContainer,
            IItemContainer secondaryItemContainer,
            ItemCommandHandler handler)
        {
            _window = window;
            _equipmentModel = equipmentModel;
            _firstItemStack = primaryItemStack;
            _secondItemStack = secondaryItemStack;
            _primaryItemContainer = primaryItemContainer;
            _secondaryItemContainer = secondaryItemContainer;
            _handler = handler;

            _primaryItemContainer.Changed += Redraw;
            _secondaryItemContainer.Changed += Redraw;

            Redraw();
        }

        private void Redraw()
        {
            _window.Render(_firstItemStack, _secondItemStack);
        }

        private void OnTransferClicked(ItemStack item, IItemContainer toContainer)
        {
            _handler.Handle(new MoveItemCommand()
            {
                FromItem = item,
                ToContainer = toContainer,
                Amount = item.Amount
            });
        }

        private void OnEquipClicked(ItemStack item, IItemContainer toContainer)
        {
            _handler.Handle(new MoveItemCommand()
            {
                FromItem = item,
                ToContainer = toContainer,
                Amount = item.Amount
            });
        }

        private void OnUnequipClicked(ItemStack item, IItemContainer toContainer)
        {
            _handler.Handle(new MoveItemCommand()
            {
                FromItem = item,
                ToContainer = toContainer,
                Amount = item.Amount
            });
        }

        public void Dispose()
        {
            _primaryItemContainer.Changed -= Redraw;
            _secondaryItemContainer.Changed -= Redraw;
        }
    }

    public sealed class ItemActionUI
    {
        public string Id;
        public string Title;
    }
}
