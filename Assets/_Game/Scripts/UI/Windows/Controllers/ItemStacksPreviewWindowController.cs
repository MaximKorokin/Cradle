using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Shared.Utils;
using Assets.CoreScripts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ItemStacksPreviewWindowController<T1, T2> : WindowControllerBase<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments<T1, T2>>
    {
        private readonly WindowManager _windowManager;
        private readonly ItemCommandHandler _handler;

        private ItemStacksPreviewWindow _window;
        private EquipmentModel _equipmentModel;
        private EquipmentSlotKey? _equipmentSlot;
        private IItemContainer<T1> _primaryItemContainer;
        private T1 _primaryContainerSlot;
        private IItemContainer<T2> _secondaryItemContainer;

        public ItemStacksPreviewWindowController(
            WindowManager windowManager,
            ItemCommandHandler handler)
        {
            _windowManager = windowManager;
            _handler = handler;
        }

        public override void Bind(ItemStacksPreviewWindow window)
        {
            _window = window;

            _window.ActionButtonClicked += ProcessAction;
        }

        public override void Initialize(ItemStacksPreviewWindowControllerArguments<T1, T2> arguments)
        {
            base.Initialize(arguments);

            _equipmentModel = arguments.EquipmentModel;
            _equipmentSlot = arguments.EquipmentSlot;
            _primaryItemContainer = arguments.PrimaryItemContainer;
            _primaryContainerSlot = arguments.PrimaryContainerSlot;
            _secondaryItemContainer = arguments.SecondaryItemContainer;

            _primaryItemContainer.Changed += Redraw;
            _secondaryItemContainer.Changed += Redraw;

            Redraw();
        }

        public override void Dispose()
        {
            _primaryItemContainer.Changed -= Redraw;
            _secondaryItemContainer.Changed -= Redraw;

            _window.ActionButtonClicked -= ProcessAction;
        }

        private void Redraw()
        {
            var primaryItem = _primaryItemContainer.Get(_primaryContainerSlot);
            var equipmentItem = _equipmentSlot != null ? _equipmentModel.Get(_equipmentSlot.Value) : null;
            _window.Render(primaryItem, equipmentItem, GetActions());
        }

        private void OnDropClicked<T>(IItemContainer<T> fromContainer, T slot, int amount)
        {
            _handler.Handle<T>(new DropItemCommand<T>()
            {
                FromContainer = fromContainer,
                FromSlot = slot,
                Amount = amount
            });
        }

        private void OnTransferClicked<T>(IItemContainer<T> fromContainer, T slot, IItemContainer toContainer, int amount)
        {
            _handler.Handle<T>(new MoveItemCommand<T>()
            {
                FromContainer = fromContainer,
                FromSlot = slot,
                ToContainer = toContainer,
                Amount = amount
            });
        }

        private void OnEquipClicked<T>(IItemContainer<T> fromContainer, T slot, EquipmentModel equipmentModel, EquipmentSlotKey equipmentSlot)
        {
            _handler.Handle<T>(new EquipFromContainerCommand<T>()
            {
                EquipmentModel = equipmentModel,
                EquipmentSlot = equipmentSlot,
                FromContainer = fromContainer,
                FromSlot = slot,
            });
        }

        private void OnUnequipClicked<T>(IItemContainer<T> toContainer, EquipmentSlotKey slot, EquipmentModel equipmentModel)
        {
            _handler.Handle<T>(new UnequipToContainerCommand()
            {
                EquipmentModel = equipmentModel,
                ToContainer = toContainer,
                SlotKey = slot
            });
        }

        private IEnumerable<ItemStackAction> GetActions()
        {
            var primaryItemNullable = _primaryItemContainer.Get(_primaryContainerSlot);
            if (primaryItemNullable == null)
            {
                SLog.Error($"Trying to get actions for item that is not found in {_primaryItemContainer} in slot {_primaryContainerSlot}");
                return Enumerable.Empty<ItemStackAction>();
            }

            var primaryItem = primaryItemNullable.Value;
            var actions = new List<ItemStackAction>
            {
                new(ItemStackActionType.Drop, "Drop")
            };

            if (_primaryItemContainer is InventoryModel &&
                _secondaryItemContainer is InventoryModel &&
                _secondaryItemContainer.PreviewAdd(primaryItem) > 0)
            {
                if (primaryItem.Amount > 1)
                {
                    actions.Add(new ItemStackAction(ItemStackActionType.TransferOne, "Transfer One"));
                    actions.Add(new ItemStackAction(ItemStackActionType.TransferHalf, "Transfer Half"));
                    actions.Add(new ItemStackAction(ItemStackActionType.TransferAll, "Transfer All"));
                }
                else
                {
                    actions.Add(new ItemStackAction(ItemStackActionType.TransferAll, "Transfer"));
                }
            }
            if ((_primaryItemContainer is EquipmentModel equipmentModel) &&
                equipmentModel == _equipmentModel &&
                _equipmentModel.Has(primaryItem.Key, 1))
            {
                actions.Add(new ItemStackAction(ItemStackActionType.Unequip, "Unequip"));
            }
            else if (_equipmentModel.CanEquip(primaryItem))
            {
                actions.Add(new ItemStackAction(ItemStackActionType.Equip, "Equip"));
            }
            return actions;
        }

        private void ProcessAction(ItemStackActionType actionType)
        {
            _windowManager.CloseTop();
            var item = _primaryItemContainer.Get(_primaryContainerSlot);
            switch (actionType)
            {
                case ItemStackActionType.Drop:
                    if (item != null)
                    {
                        OnDropClicked(_primaryItemContainer, _primaryContainerSlot, item.Value.Amount);
                    }
                    break;
                case ItemStackActionType.TransferOne:
                    OnTransferClicked(_primaryItemContainer, _primaryContainerSlot, _secondaryItemContainer, 1);
                    break;
                case ItemStackActionType.TransferHalf:
                    if (item != null)
                    {
                        var halfAmount = (int)Math.Ceiling(item.Value.Amount / 2f);
                        OnTransferClicked(_primaryItemContainer, _primaryContainerSlot, _secondaryItemContainer, halfAmount);
                    }
                    break;
                case ItemStackActionType.TransferAll:
                    if (item != null)
                    {
                        OnTransferClicked(_primaryItemContainer, _primaryContainerSlot, _secondaryItemContainer, item.Value.Amount);
                    }
                    break;
                case ItemStackActionType.Equip:
                    OnEquipClicked(_primaryItemContainer, _primaryContainerSlot, _equipmentModel, _equipmentSlot.Value);
                    break;
                case ItemStackActionType.Unequip:
                    if (_primaryContainerSlot is EquipmentSlotKey key)
                    {
                        OnUnequipClicked(_secondaryItemContainer, key, _equipmentModel);
                    }
                    break;
            }
        }
    }

    public sealed class ItemStackAction
    {
        public ItemStackActionType Type;
        public string Title;

        public ItemStackAction(ItemStackActionType type, string title)
        {
            Type = type;
            Title = title;
        }
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
