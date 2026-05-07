using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataFormatters;
using System.Collections.Generic;

namespace Assets._Game.Scripts.UI.Windows.Controllers.ItemPreview
{
    public sealed class InventoryToShopPreviewStrategy : IItemStacksPreviewStrategy
    {
        private readonly WindowManager _windowManager;
        private readonly IPlayerProvider _playerProvider;
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly ItemStackFormatter _itemStackFormatter;
        private readonly ShopModel _shopModel;
        private readonly long _inventorySlot;
        private readonly float _sellCoefficient;
        private readonly EquipmentSlotKey? _equipmentSlot;

        private InventoryModel _inventoryModel;
        private EquipmentModel _equipmentModel;
        private ItemStacksPreviewWindow _window;

        public InventoryToShopPreviewStrategy(
            WindowManager windowManager,
            IPlayerProvider playerProvider,
            ItemContainerResolver itemContainerResolver,
            ItemStackFormatter itemStackFormatter,
            ShopModel shopModel,
            long inventorySlot,
            float sellCoefficient,
            EquipmentSlotKey? equipmentSlot)
        {
            _windowManager = windowManager;
            _playerProvider = playerProvider;
            _itemContainerResolver = itemContainerResolver;
            _itemStackFormatter = itemStackFormatter;
            _shopModel = shopModel;
            _inventorySlot = inventorySlot;
            _sellCoefficient = sellCoefficient;
            _equipmentSlot = equipmentSlot;
        }

        public void Initialize(ItemStacksPreviewWindow window)
        {
            _window = window;
            _inventoryModel = _itemContainerResolver.ResolveInventory(_playerProvider.Player, ItemContainerId.Inventory);
            _equipmentModel = _itemContainerResolver.ResolveEquipment(_playerProvider.Player);

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
                    if (item.Value.Definition.TryGetTrait<PriceTrait>(out var priceTrait))
                    {
                        _windowManager.ShowAmountPickerIfNeeded(item.Value.Amount, item.Value.Amount, amount =>
                        {
                            int price = (int)(priceTrait.BasePrice * _sellCoefficient * amount);
                            PublishItemCommand(new SellToShopCommand(_shopModel, _inventorySlot, amount, price));
                        });
                    }
                    break;
                case ItemStackActionType.Drop:
                    PublishItemCommand(new DropItemCommand(ItemContainerId.Inventory, _inventorySlot, item.Value.Amount));
                    break;
            }
        }

        private void PublishItemCommand(IItemCommand command)
        {
            _windowManager.CloseWindow(_window);

            var entity = _playerProvider.Player;
            entity.Publish(new ItemCommandRequest(command));
        }

        private IEnumerable<ItemStackAction> GetActions(ItemStackSnapshot item)
        {
            var actions = new List<ItemStackAction>();

            // Selling to shop
            if (item.Definition.TryGetTrait<PriceTrait>(out var priceTrait))
            {
                int sellPrice = (int)(priceTrait.BasePrice * _sellCoefficient);
                actions.Add(new ItemStackAction(ItemStackActionType.Sell, $"Sell ({sellPrice}g)"));
            }

            actions.Add(new ItemStackAction(ItemStackActionType.Drop, "Drop"));

            return actions;
        }
    }
}
