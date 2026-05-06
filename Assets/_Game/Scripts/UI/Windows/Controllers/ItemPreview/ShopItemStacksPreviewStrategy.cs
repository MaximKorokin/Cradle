using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
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
        private readonly ItemStackFormatter _itemStackFormatter;
        private readonly ShopModel _shopModel;
        private readonly long _shopSlot;
        private readonly bool _isBuying;
        private readonly float _buyCoefficient;

        private ItemStacksPreviewWindow _window;

        public ShopItemStacksPreviewStrategy(
            WindowManager windowManager,
            IPlayerProvider playerProvider,
            ItemStackFormatter itemStackFormatter,
            ShopModel shopModel,
            long shopSlot,
            bool isBuying,
            float buyCoefficient)
        {
            _windowManager = windowManager;
            _playerProvider = playerProvider;
            _itemStackFormatter = itemStackFormatter;
            _shopModel = shopModel;
            _shopSlot = shopSlot;
            _isBuying = isBuying;
            _buyCoefficient = buyCoefficient;
        }

        public void Initialize(ItemStacksPreviewWindow window)
        {
            _window = window;
            _shopModel.Changed += OnShopChanged;
        }

        public void Cleanup(ItemStacksPreviewWindow window)
        {
            _shopModel.Changed -= OnShopChanged;
        }

        private void OnShopChanged()
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
                        if (item.Value.Definition.MaxAmount == 1)
                        {
                            int price = (int)(priceTrait.BasePrice * _buyCoefficient);
                            PublishItemCommand(new BuyFromShopCommand(_shopModel, _shopSlot, 1, price));
                        }
                        else
                        {
                            _windowManager.ShowAmountPicker(1, item.Value.Amount, amount => {
                                int price = (int)(priceTrait.BasePrice * _buyCoefficient * amount);
                                PublishItemCommand(new BuyFromShopCommand(_shopModel, _shopSlot, amount, price));
                            });
                        }
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
                int buyPrice = (int)(priceTrait.BasePrice * _buyCoefficient);
                actions.Add(new ItemStackAction(ItemStackActionType.Buy, $"Buy ({buyPrice}g)"));
            }

            return actions;
        }
    }
}
