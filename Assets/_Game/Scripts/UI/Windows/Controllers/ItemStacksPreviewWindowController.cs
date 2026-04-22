using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ItemStacksPreviewWindowController : WindowControllerBase<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments>
    {
        private readonly WindowManager _windowManager;
        private readonly IPlayerProvider _playerProvider;
        private readonly ItemContainerResolver _itemContainerResolver;

        private ItemStacksPreviewWindow _window;
        private EquipmentSlotKey? _equipmentSlot;
        private long _primaryContainerSlot;
        private ItemContainerId _primaryContainerId;
        private ItemContainerId _secondaryContainerId;
        private IItemContainer _primaryContainer;
        private IItemContainer _secondaryContainer;
        private EquipmentModel _equipmentModel;

        public ItemStacksPreviewWindowController(
            WindowManager windowManager,
            IPlayerProvider playerProvider,
            ItemContainerResolver itemContainerResolver)
        {
            _windowManager = windowManager;
            _playerProvider = playerProvider;
            _itemContainerResolver = itemContainerResolver;
        }

        public override void Bind(ItemStacksPreviewWindow window)
        {
            _window = window;

            _window.ActionButtonClicked += ProcessAction;
        }

        public override void Initialize(ItemStacksPreviewWindowControllerArguments arguments)
        {
            base.Initialize(arguments);

            _equipmentSlot = arguments.EquipmentSlot;
            _primaryContainerSlot = arguments.PrimaryContainerSlot;
            _primaryContainerId = arguments.PrimaryContainerId;
            _secondaryContainerId = arguments.SecondaryContainerId;
            _primaryContainer = _itemContainerResolver.ResolveContainer(_playerProvider.Player, arguments.PrimaryContainerId);
            _secondaryContainer = _itemContainerResolver.ResolveContainer(_playerProvider.Player, arguments.SecondaryContainerId);
            _equipmentModel = _itemContainerResolver.ResolveEquipment(_playerProvider.Player);

            _primaryContainer.Changed += Redraw;
            _secondaryContainer.Changed += Redraw;

            Redraw();
        }

        public override void Unbind()
        {
            _primaryContainer.Changed -= Redraw;
            _secondaryContainer.Changed -= Redraw;

            _window.ActionButtonClicked -= ProcessAction;
        }

        private void Redraw()
        {
            var primaryItem = _primaryContainer.Get(_primaryContainerSlot);

            // Primary item disappeared, nothing to preview
            if (primaryItem == null)
            {
                _windowManager.CloseWindow(_window);
                return;
            }

            var equipmentItem = _equipmentSlot != null ? _equipmentModel.Get(_equipmentSlot.Value) : null;
            var actions = GetActions();
            _window.Render(primaryItem, equipmentItem, actions);
        }

        private void PublishItemCommand(IItemCommand command)
        {
            _windowManager.CloseWindow(_window);

            var entity = _playerProvider.Player;
            entity.Publish(new ItemCommandRequest(command));
        }

        private IEnumerable<ItemStackAction> GetActions()
        {
            var primaryItemNullable = _primaryContainer.Get(_primaryContainerSlot);
            if (primaryItemNullable == null)
            {
                SLog.Error($"Trying to get actions for item that is not found in {_primaryContainer} in slot {_primaryContainerSlot}");
                return Array.Empty<ItemStackAction>();
            }

            var primaryItem = primaryItemNullable.Value;
            var actions = new List<ItemStackAction>
            {
                new(ItemStackActionType.Drop, "Drop")
            };

            if (_primaryContainer is InventoryModel &&
                _secondaryContainer is InventoryModel &&
                _secondaryContainer.PreviewAdd(primaryItem) > 0)
            {
                actions.Add(new ItemStackAction(ItemStackActionType.Transfer, "Transfer"));
            }

            if ((_primaryContainer is EquipmentModel equipmentModel) &&
                equipmentModel == _equipmentModel &&
                _equipmentModel.Has(primaryItem.Key, 1))
            {
                actions.Add(new ItemStackAction(ItemStackActionType.Unequip, "Unequip"));
            }
            else if (_equipmentModel.CanEquip(primaryItem))
            {
                actions.Add(new ItemStackAction(ItemStackActionType.Equip, "Equip"));
            }

            if (primaryItem.GetTrait<UsableTrait>() != null && primaryItem.GetTraits<FunctionalItemTraitBase>().Any(t => t.Triggers.HasFlag(ItemTrigger.OnUse)))
            {
                actions.Add(new ItemStackAction(ItemStackActionType.Use, "Use"));
            }
            return actions;
        }

        private void ProcessAction(ItemStackActionType actionType)
        {
            var item = _primaryContainer.Get(_primaryContainerSlot);

            switch (actionType)
            {
                case ItemStackActionType.Drop:
                    if (item != null)
                    {
                        PublishItemCommand(new DropItemCommand(_primaryContainerId, _primaryContainerSlot, item.Value.Amount));
                    }
                    break;
                case ItemStackActionType.Transfer:
                    if (item != null)
                    {
                        // If there's only one item, transfer it directly, otherwise show amount picker
                        if (item.Value.Amount == 1)
                        {
                            PublishItemCommand(new TransferItemCommand(_primaryContainerId, _primaryContainerSlot, _secondaryContainerId, 1));
                        }
                        else
                        {
                            AmountPickerWindow amountPickerWindow = null;
                            amountPickerWindow = _windowManager.InstantiateWindow<AmountPickerWindow, AmountPickerWindowControllerArguments>(
                                new(1,
                                    item.Value.Amount,
                                    amount =>
                                    {
                                        PublishItemCommand(new TransferItemCommand(_primaryContainerId, _primaryContainerSlot, _secondaryContainerId, amount));
                                        _windowManager.CloseWindow(amountPickerWindow);
                                    }));
                        }
                    }
                    break;
                case ItemStackActionType.Equip:
                    PublishItemCommand(new EquipFromContainerCommand(_primaryContainerId, _primaryContainerSlot, _equipmentSlot.Value.ToInt64()));
                    break;
                case ItemStackActionType.Unequip:
                    PublishItemCommand(new UnequipToContainerCommand(_secondaryContainerId, _primaryContainerSlot));
                    break;
                case ItemStackActionType.Use:
                    PublishItemCommand(new UseItemCommand(_primaryContainerId, _primaryContainerSlot, true));
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
        Transfer,
        Equip,
        Unequip,
        Use,
    }
}
