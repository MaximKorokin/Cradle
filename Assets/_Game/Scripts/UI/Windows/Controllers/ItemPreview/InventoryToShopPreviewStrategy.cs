using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataFormatters;
using System.Collections.Generic;

namespace Assets._Game.Scripts.UI.Windows.Controllers.ItemPreview
{
    public sealed class InventoryToShopPreviewStrategy : IItemStacksPreviewStrategy
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly WindowManager _windowManager;
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly ItemStackFormatter _itemStackFormatter;
        private readonly ItemContainerPath _shopContainerPath;
        private readonly ItemContainerPath _inventoryContainerPath;
        private readonly ItemContainerPath _equipmentContainerPath;
        private readonly long _inventorySlot;
        private readonly float _sellCoefficient;
        private readonly EquipmentSlotKey? _equipmentSlot;

        private InventoryModel _inventoryModel;
        private EquipmentModel _equipmentModel;
        private ItemStacksPreviewWindow _window;

        public InventoryToShopPreviewStrategy(
            IGlobalEventBus globalEventBus,
            WindowManager windowManager,
            ItemContainerResolver itemContainerResolver,
            ItemStackFormatter itemStackFormatter,
            ItemContainerPath shopContainerPath,
            ItemContainerPath inventoryContainerPath,
            ItemContainerPath equipmentContainerPath,
            long inventorySlot,
            float sellCoefficient,
            EquipmentSlotKey? equipmentSlot)
        {
            _globalEventBus = globalEventBus;
            _windowManager = windowManager;
            _itemContainerResolver = itemContainerResolver;
            _itemStackFormatter = itemStackFormatter;
            _shopContainerPath = shopContainerPath;
            _inventoryContainerPath = inventoryContainerPath;
            _equipmentContainerPath = equipmentContainerPath;
            _inventorySlot = inventorySlot;
            _sellCoefficient = sellCoefficient;
            _equipmentSlot = equipmentSlot;
        }

        public void Initialize(ItemStacksPreviewWindow window)
        {
            _window = window;

            _inventoryModel = _itemContainerResolver.ResolveInventory(_inventoryContainerPath);
            _equipmentModel = _itemContainerResolver.ResolveEquipment(_equipmentContainerPath);

            _inventoryModel.Changed += OnInventoryChanged;
            _equipmentModel.Changed += OnEquipmentChanged;
        }

        public void Cleanup(ItemStacksPreviewWindow window)
        {
            _inventoryModel.Changed -= OnInventoryChanged;
            _equipmentModel.Changed -= OnEquipmentChanged;
        }

        private void OnInventoryChanged()
        {
            Redraw(_window);
        }

        private void OnEquipmentChanged()
        {
            Redraw(_window);
        }

        public void Redraw(ItemStacksPreviewWindow window)
        {
            var item = _inventoryModel.Get(InventorySlot.FromInt64(_inventorySlot));

            if (!item.HasValue)
            {
                _windowManager.CloseWindow(window);
                return;
            }

            var actions = GetActions(item.Value);

            var equippedItem = _equipmentSlot != null ? _equipmentModel.Get(_equipmentSlot.Value) : null;

            if (equippedItem.HasValue)
                window.Render(_itemStackFormatter.FormatData(item.Value), _itemStackFormatter.FormatData(equippedItem.Value), actions);
            else
                window.Render(_itemStackFormatter.FormatData(item.Value), actions);
        }

        public void ProcessAction(ItemStackActionType actionType)
        {
            var item = _inventoryModel.Get(InventorySlot.FromInt64(_inventorySlot));
            if (!item.HasValue) return;

            switch (actionType)
            {
                case ItemStackActionType.Sell:
                    if (item.Value.Definition.TryGetSellPrice(_sellCoefficient, out var sellPricePerUnit))
                    {
                        _windowManager.ShowAmountPickerIfNeeded(item.Value.Amount, item.Value.Amount, amount =>
                        {
                            PublishItemCommand(new SellToShopCommand(_shopContainerPath, _inventoryContainerPath, _inventorySlot, amount, sellPricePerUnit * amount));
                        });
                    }
                    break;
                case ItemStackActionType.Drop:
                    PublishItemCommand(new DropItemCommand(_inventoryContainerPath, _inventorySlot, item.Value.Amount));
                    break;
            }
        }

        private void PublishItemCommand(IItemCommand command)
        {
            _windowManager.CloseWindow(_window);

            _globalEventBus.Publish(new ItemCommandRequest(command));
        }

        private IEnumerable<ItemStackAction> GetActions(ItemStackSnapshot item)
        {
            var actions = new List<ItemStackAction>();

            // Selling to shop
            if (item.Definition.TryGetSellPrice(_sellCoefficient, out var sellPrice))
            {
                actions.Add(new ItemStackAction(ItemStackActionType.Sell, $"Sell ({sellPrice}g)"));
            }

            actions.Add(new ItemStackAction(ItemStackActionType.Drop, "Drop"));

            return actions;
        }
    }
}
