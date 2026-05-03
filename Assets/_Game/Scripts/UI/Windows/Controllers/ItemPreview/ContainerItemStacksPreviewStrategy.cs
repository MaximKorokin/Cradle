using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.Shared.Utils;
using Assets._Game.Scripts.UI.DataFormatters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.Windows.Controllers.ItemPreview
{
    public sealed class ContainerItemStacksPreviewStrategy : IItemStacksPreviewStrategy
    {
        private readonly WindowManager _windowManager;
        private readonly IPlayerProvider _playerProvider;
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly ItemStackFormatter _itemStackFormatter;
        private readonly EquipmentSlotKey? _equipmentSlot;
        private readonly long _primaryContainerSlot;
        private readonly ItemContainerId _primaryContainerId;
        private readonly ItemContainerId _secondaryContainerId;

        private IItemContainer _primaryContainer;
        private IItemContainer _secondaryContainer;
        private EquipmentModel _equipmentModel;
        private ItemStacksPreviewWindow _window;

        public ContainerItemStacksPreviewStrategy(
            WindowManager windowManager,
            IPlayerProvider playerProvider,
            ItemContainerResolver itemContainerResolver,
            ItemStackFormatter itemStackFormatter,
            EquipmentSlotKey? equipmentSlot,
            long primaryContainerSlot,
            ItemContainerId primaryContainerId,
            ItemContainerId secondaryContainerId)
        {
            _windowManager = windowManager;
            _playerProvider = playerProvider;
            _itemContainerResolver = itemContainerResolver;
            _itemStackFormatter = itemStackFormatter;
            _equipmentSlot = equipmentSlot;
            _primaryContainerSlot = primaryContainerSlot;
            _primaryContainerId = primaryContainerId;
            _secondaryContainerId = secondaryContainerId;
        }

        public void Initialize(ItemStacksPreviewWindow window)
        {
            _window = window;
            _primaryContainer = _itemContainerResolver.ResolveContainer(_playerProvider.Player, _primaryContainerId);
            _secondaryContainer = _itemContainerResolver.ResolveContainer(_playerProvider.Player, _secondaryContainerId);
            _equipmentModel = _itemContainerResolver.ResolveEquipment(_playerProvider.Player);

            _primaryContainer.Changed += OnContainerChanged;
            _secondaryContainer.Changed += OnContainerChanged;
        }

        public void Cleanup(ItemStacksPreviewWindow window)
        {
            _primaryContainer.Changed -= OnContainerChanged;
            _secondaryContainer.Changed -= OnContainerChanged;
        }

        private void OnContainerChanged()
        {
            Redraw(_window);
        }

        public void Redraw(ItemStacksPreviewWindow window)
        {
            var primaryItem = _primaryContainer.Get(_primaryContainerSlot);

            if (primaryItem == null)
            {
                _windowManager.CloseWindow(window);
                return;
            }

            var equipmentItem = _equipmentSlot != null ? _equipmentModel.Get(_equipmentSlot.Value) : null;
            var actions = GetActions();

            if (equipmentItem != null)
                window.Render(_itemStackFormatter.FormatData(primaryItem.Value), _itemStackFormatter.FormatData(equipmentItem.Value), actions);
            else
                window.Render(_itemStackFormatter.FormatData(primaryItem.Value), actions);
        }

        public void ProcessAction(ItemStackActionType actionType)
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
                        if (item.Value.Amount == 1)
                        {
                            PublishItemCommand(new TransferItemCommand(_primaryContainerId, _primaryContainerSlot, _secondaryContainerId, 1));
                        }
                        else
                        {
                            _windowManager.ShowAmountPicker(1, item.Value.Amount, amount =>
                            {
                                PublishItemCommand(new TransferItemCommand(_primaryContainerId, _primaryContainerSlot, _secondaryContainerId, amount));
                            });
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
            var actions = new List<ItemStackAction>();

            // Standard actions
            actions.Add(new(ItemStackActionType.Drop, "Drop"));

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
    }
}
