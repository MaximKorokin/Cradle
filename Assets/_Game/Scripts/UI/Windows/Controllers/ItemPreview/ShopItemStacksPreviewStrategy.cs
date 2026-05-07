using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataFormatters;
using System.Collections.Generic;

namespace Assets._Game.Scripts.UI.Windows.Controllers.ItemPreview
{
    public sealed class ShopItemStacksPreviewStrategy : IItemStacksPreviewStrategy
    {
        private readonly WindowManager _windowManager;
        private readonly IPlayerProvider _playerProvider;
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly ItemStackFormatter _itemStackFormatter;
        private readonly ShopModel _shopModel;
        private readonly long _shopSlot;
        private readonly bool _isBuying;
        private readonly float _buyCoefficient;
        private readonly float _sellCoefficient;
        private readonly EquipmentSlotKey? _equipmentSlot;

        private EquipmentModel _equipmentModel;
        private ItemStacksPreviewWindow _window;

        public ShopItemStacksPreviewStrategy(
            WindowManager windowManager,
            IPlayerProvider playerProvider,
            ItemContainerResolver itemContainerResolver,
            ItemStackFormatter itemStackFormatter,
            ShopModel shopModel,
            long shopSlot,
            bool isBuying,
            float buyCoefficient,
            float sellCoefficient,
            EquipmentSlotKey? equipmentSlot)
        {
            _windowManager = windowManager;
            _playerProvider = playerProvider;
            _itemContainerResolver = itemContainerResolver;
            _itemStackFormatter = itemStackFormatter;
            _shopModel = shopModel;
            _shopSlot = shopSlot;
            _isBuying = isBuying;
            _buyCoefficient = buyCoefficient;
            _sellCoefficient = sellCoefficient;
            _equipmentSlot = equipmentSlot;
        }

        public void Initialize(ItemStacksPreviewWindow window)
        {
            _window = window;
            _equipmentModel = _itemContainerResolver.ResolveEquipment(_playerProvider.Player);
            _shopModel.Changed += OnShopChanged;
            _equipmentModel.Changed += OnEquipmentChanged;
        }

        public void Cleanup(ItemStacksPreviewWindow window)
        {
            _shopModel.Changed -= OnShopChanged;
            _equipmentModel.Changed -= OnEquipmentChanged;
        }

        private void OnShopChanged()
        {
            Redraw(_window);
        }

        private void OnEquipmentChanged()
        {
            Redraw(_window);
        }

        public void Redraw(ItemStacksPreviewWindow window)
        {
            var item = _shopModel.Get(ShopSlot.FromInt64(_shopSlot));

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
            var item = _shopModel.Get(ShopSlot.FromInt64(_shopSlot));
            if (!item.HasValue) return;

            switch (actionType)
            {
                case ItemStackActionType.Buy:
                    if (item.Value.Definition.TryGetTrait<PriceTrait>(out var priceTrait))
                    {
                        _windowManager.ShowAmountPickerIfNeeded(item.Value.Amount, item.Value.Amount, amount => {
                             int price = _shopModel.TryGetBuyPrice(ShopSlot.FromInt64(_shopSlot), _buyCoefficient, _sellCoefficient, out var buyPrice) ? buyPrice * amount : 0;
                             PublishItemCommand(new BuyFromShopCommand(_shopModel, _shopSlot, amount, price));
                        });
                    }
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

            if (_isBuying && item.Definition.TryGetTrait<PriceTrait>(out var priceTrait))
            {
                int price = _shopModel.TryGetBuyPrice(ShopSlot.FromInt64(_shopSlot), _buyCoefficient, _sellCoefficient, out var buyPrice) ? buyPrice : 0;
                actions.Add(new ItemStackAction(ItemStackActionType.Buy, $"Buy ({price}g)"));
            }

            return actions;
        }
    }
}
