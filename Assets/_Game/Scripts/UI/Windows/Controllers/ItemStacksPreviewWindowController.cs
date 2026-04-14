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
    public sealed class ItemStacksPreviewWindowController<T1, T2> : WindowControllerBase<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments<T1, T2>>
        where T1 : struct, IContainerSlot
        where T2 : struct, IContainerSlot
    {
        private readonly WindowManager _windowManager;
        private readonly IPlayerProvider _playerProvider;

        private ItemStacksPreviewWindow _window;
        private EquipmentModel _equipmentModel;
        private EquipmentSlotKey? _equipmentSlot;
        private IItemContainer<T1> _primaryItemContainer;
        private T1 _primaryContainerSlot;
        private IItemContainer<T2> _secondaryItemContainer;
        private ItemContainerId _primaryContainerId;
        private ItemContainerId _secondaryContainerId;

        public ItemStacksPreviewWindowController(
            WindowManager windowManager,
            IPlayerProvider playerProvider)
        {
            _windowManager = windowManager;
            _playerProvider = playerProvider;
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
            _primaryContainerId = arguments.PrimaryContainerId;
            _secondaryContainerId = arguments.SecondaryContainerId;

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
            var entity = _playerProvider.Player;
            entity.Publish(new ItemCommandRequest(command));
        }

        private IEnumerable<ItemStackAction> GetActions()
        {
            var primaryItemNullable = _primaryItemContainer.Get(_primaryContainerSlot);
            if (primaryItemNullable == null)
            {
                SLog.Error($"Trying to get actions for item that is not found in {_primaryItemContainer} in slot {_primaryContainerSlot}");
                return Array.Empty<ItemStackAction>();
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

            if (primaryItem.GetTrait<UsableTrait>() != null && primaryItem.GetTraits<FunctionalItemTraitBase>().Any(t => t.Triggers.HasFlag(ItemTrigger.OnUse)))
            {
                actions.Add(new ItemStackAction(ItemStackActionType.Use, "Use"));
            }
            return actions;
        }

        private void ProcessAction(ItemStackActionType actionType)
        {
            _windowManager.CloseWindow(_window);
            var item = _primaryItemContainer.Get(_primaryContainerSlot);
            var primarySlot = ContainerSlotConverter.ToInt64(_primaryContainerSlot);

            switch (actionType)
            {
                case ItemStackActionType.Drop:
                    if (item != null)
                    {
                        PublishItemCommand(new DropItemCommand(_primaryContainerId, primarySlot, item.Value.Amount));
                    }
                    break;
                case ItemStackActionType.TransferOne:
                    PublishItemCommand(new TransferItemCommand(_primaryContainerId, primarySlot, _secondaryContainerId, 1));
                    break;
                case ItemStackActionType.TransferHalf:
                    if (item != null)
                    {
                        var halfAmount = (int)Math.Ceiling(item.Value.Amount / 2f);
                        PublishItemCommand(new TransferItemCommand(_primaryContainerId, primarySlot, _secondaryContainerId, halfAmount));
                    }
                    break;
                case ItemStackActionType.TransferAll:
                    if (item != null)
                    {
                        PublishItemCommand(new TransferItemCommand(_primaryContainerId, primarySlot, _secondaryContainerId, item.Value.Amount));
                    }
                    break;
                case ItemStackActionType.Equip:
                    PublishItemCommand(new EquipFromContainerCommand(_primaryContainerId, primarySlot, _equipmentSlot.Value.ToInt64()));
                    break;
                case ItemStackActionType.Unequip:
                    PublishItemCommand(new UnequipToContainerCommand(_secondaryContainerId, primarySlot));
                    break;
                case ItemStackActionType.Use:
                    PublishItemCommand(new UseItemCommand(_primaryContainerId, primarySlot, true));
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
        Unequip,
        Use,
    }
}
