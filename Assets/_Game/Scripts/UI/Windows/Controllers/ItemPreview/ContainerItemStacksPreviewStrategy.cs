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
        private readonly IGlobalEventBus _globalEventBus;
        private readonly WindowManager _windowManager;
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly ItemStackFormatter _itemStackFormatter;
        private readonly EquipmentSlotKey? _equipmentSlot;
        private readonly long _primaryContainerSlot;
        private readonly ItemContainerPath _equipmentContainerPath;
        private readonly ItemContainerPath _primaryContainerPath;
        private readonly ItemContainerPath _secondaryContainerPath;

        private IItemContainer _primaryContainer;
        private IItemContainer _secondaryContainer;
        private EquipmentModel _equipmentModel;
        private ItemStacksPreviewWindow _window;

        public ContainerItemStacksPreviewStrategy(
            IGlobalEventBus globalEventBus,
            WindowManager windowManager,
            ItemContainerResolver itemContainerResolver,
            ItemStackFormatter itemStackFormatter,
            EquipmentSlotKey? equipmentSlot,
            long primaryContainerSlot,
            ItemContainerPath equipmentContainerPath,
            ItemContainerPath primaryContainerPath,
            ItemContainerPath secondaryContainerPath)
        {
            _globalEventBus = globalEventBus;
            _windowManager = windowManager;
            _itemContainerResolver = itemContainerResolver;
            _itemStackFormatter = itemStackFormatter;
            _equipmentSlot = equipmentSlot;
            _primaryContainerSlot = primaryContainerSlot;
            _equipmentContainerPath = equipmentContainerPath;
            _primaryContainerPath = primaryContainerPath;
            _secondaryContainerPath = secondaryContainerPath;
        }

        public void Initialize(ItemStacksPreviewWindow window)
        {
            _window = window;
            _primaryContainer = _itemContainerResolver.ResolveContainer(_primaryContainerPath);
            _secondaryContainer = _itemContainerResolver.ResolveContainer(_secondaryContainerPath);
            _equipmentModel = _itemContainerResolver.ResolveEquipment(_equipmentContainerPath);

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
                        PublishItemCommand(new DropItemCommand(_primaryContainerPath, _primaryContainerSlot, item.Value.Amount));
                    }
                    break;
                case ItemStackActionType.Transfer:
                    if (item != null)
                    {
                        _windowManager.ShowAmountPickerIfNeeded(item.Value.Amount, item.Value.Amount, amount =>
                        {
                            PublishItemCommand(new TransferItemCommand(_primaryContainerPath, _primaryContainerSlot, _secondaryContainerPath, amount));
                        });
                    }
                    break;
                case ItemStackActionType.Equip:
                    PublishItemCommand(new EquipFromContainerCommand(_primaryContainerPath, _primaryContainerSlot, _equipmentContainerPath, _equipmentSlot.Value.ToInt64()));
                    break;
                case ItemStackActionType.Unequip:
                    PublishItemCommand(new UnequipToContainerCommand(_secondaryContainerPath, _equipmentContainerPath, _primaryContainerSlot));
                    break;
                case ItemStackActionType.Use:
                    PublishItemCommand(new UseItemCommand(_primaryContainerPath, _primaryContainerSlot, true));
                    break;
            }
        }

        private void PublishItemCommand(IItemCommand command)
        {
            _windowManager.CloseWindow(_window);

            _globalEventBus.Publish(new ItemCommandRequest(command));
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
