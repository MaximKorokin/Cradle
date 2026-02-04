using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Shared.Extensions;
using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class ItemStacksPreviewWindowController : IDisposable
    {
        private readonly WindowManager _windowManager;
        private readonly ItemStacksPreviewWindow _window;
        private readonly ItemCommandHandler _handler;
        private readonly EquipmentModel _equipmentModel;
        private readonly ItemStack _primaryItemStack;
        private readonly ItemStack _secondaryItemStack;
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
            _windowManager = windowManager;
            _window = window;
            _equipmentModel = equipmentModel;
            _primaryItemStack = primaryItemStack;
            _secondaryItemStack = secondaryItemStack;
            _primaryItemContainer = primaryItemContainer;
            _secondaryItemContainer = secondaryItemContainer;
            _handler = handler;

            _primaryItemContainer.Changed += Redraw;
            _secondaryItemContainer.Changed += Redraw;

            _window.ActionButtonClicked += ProcessAction;

            Redraw();
        }

        public void Dispose()
        {
            _primaryItemContainer.Changed -= Redraw;
            _secondaryItemContainer.Changed -= Redraw;

            _window.ActionButtonClicked -= ProcessAction;
        }

        private void Redraw()
        {
            _window.Render(_primaryItemStack, _secondaryItemStack, GetActions());
        }

        private void OnDropClicked(ItemStack item, IItemContainer fromContainer, int amount)
        {
            _handler.Handle(new DropItemCommand()
            {
                Item = item,
                FromContainer = fromContainer,
                Amount = amount
            });
        }

        private void OnTransferClicked(ItemStack item, IItemContainer toContainer, int amount)
        {
            _handler.Handle(new MoveItemCommand()
            {
                FromItem = item,
                ToContainer = toContainer,
                Amount = amount
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

        private IEnumerable<ItemStackAction> GetActions()
        {
            var actions = new List<ItemStackAction>();
            actions.Add(new ItemStackAction()
            {
                Type = ItemStackActionType.Drop,
                Title = "Drop"
            });
            if (_primaryItemStack.GetEquipmentSlotType() == EquipmentSlotType.None)
            {
                if (_primaryItemContainer is InventoryModel &&
                    _secondaryItemContainer is InventoryModel &&
                    _primaryItemContainer.CanPut(_primaryItemStack))
                {
                    if (_primaryItemStack.Amount > 1)
                    {
                        actions.Add(new ItemStackAction()
                        {
                            Type = ItemStackActionType.TransferOne,
                            Title = "Transfer One"
                        });
                        actions.Add(new ItemStackAction()
                        {
                            Type = ItemStackActionType.TransferHalf,
                            Title = "Transfer Half"
                        });
                        actions.Add(new ItemStackAction()
                        {
                            Type = ItemStackActionType.TransferAll,
                            Title = "Transfer All"
                        });
                    }
                    else
                    {
                        actions.Add(new ItemStackAction()
                        {
                            Type = ItemStackActionType.TransferAll,
                            Title = "Transfer"
                        });
                    }
                }
            }
            else
            {
                if (_equipmentModel.Contains(_primaryItemStack))
                {
                    actions.Add(new ItemStackAction()
                    {
                        Type = ItemStackActionType.Unequip,
                        Title = "Unequip"
                    });
                }
                else if (_equipmentModel.CanPut(_primaryItemStack))
                {
                    actions.Add(new ItemStackAction()
                    {
                        Type = ItemStackActionType.Equip,
                        Title = "Equip"
                    });
                }
            }
            return actions;
        }

        private void ProcessAction(ItemStackActionType actionType)
        {
            switch (actionType)
            {
                case ItemStackActionType.Drop:
                    OnDropClicked(_primaryItemStack, _primaryItemContainer, _primaryItemStack.Amount);
                    break;
                case ItemStackActionType.TransferOne:
                    OnTransferClicked(_primaryItemStack, _secondaryItemContainer, 1);
                    break;
                case ItemStackActionType.TransferHalf:
                    var halfAmount = (int)Math.Ceiling(_primaryItemStack.Amount / 2f);
                    OnTransferClicked(_primaryItemStack, _secondaryItemContainer, halfAmount);
                    break;
                case ItemStackActionType.TransferAll:
                    OnTransferClicked(_primaryItemStack, _secondaryItemContainer, _primaryItemStack.Amount);
                    break;
                case ItemStackActionType.Equip:
                    OnEquipClicked(_primaryItemStack, _equipmentModel);
                    break;
                case ItemStackActionType.Unequip:
                    OnUnequipClicked(_primaryItemStack, _equipmentModel);
                    break;
            }

            _windowManager.CloseTop();
        }
    }

    public sealed class ItemStackAction
    {
        public ItemStackActionType Type;
        public string Title;
    }

    public enum ItemStackActionType
    {
        Drop,
        TransferAll,
        TransferOne,
        TransferHalf,
        Equip,
        Unequip
    }
}
