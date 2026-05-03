using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.UI.DataFormatters;
using Assets._Game.Scripts.UI.Windows.Controllers.ItemPreview;

namespace Assets._Game.Scripts.UI.Windows.Shared
{
    public sealed class ShopPreviewInputProcessor
    {
        private readonly WindowManager _windowManager;
        private readonly IPlayerProvider _playerProvider;
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly ItemStackFormatter _itemStackFormatter;
        private readonly ShopModel _shopModel;
        private readonly float _buyCoefficient;
        private readonly float _sellCoefficient;

        public ShopPreviewInputProcessor(
            WindowManager windowManager,
            IPlayerProvider playerProvider,
            ItemContainerResolver itemContainerResolver,
            ItemStackFormatter itemStackFormatter,
            ShopModel shopModel,
            float buyCoefficient,
            float sellCoefficient)
        {
            _windowManager = windowManager;
            _playerProvider = playerProvider;
            _itemContainerResolver = itemContainerResolver;
            _itemStackFormatter = itemStackFormatter;
            _shopModel = shopModel;
            _buyCoefficient = buyCoefficient;
            _sellCoefficient = sellCoefficient;
        }

        public void OnShopSlotClick(ShopSlot slot)
        {
            var item = _shopModel.Get(slot);
            if (!item.HasValue) return;

            var strategy = new ShopItemStacksPreviewStrategy(
                _windowManager,
                _playerProvider,
                _itemStackFormatter,
                _shopModel,
                slot.ToInt64(),
                isBuying: true,
                _buyCoefficient);

            _windowManager.InstantiateWindow<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments>(
                new ItemStacksPreviewWindowControllerArguments(
                    null,
                    slot.ToInt64(),
                    ItemContainerId.Shop,
                    ItemContainerId.Inventory,
                    ItemStacksPreviewMode.Shop,
                    strategy));
        }

        public void OnInventorySlotClick(InventorySlot slot)
        {
            var inventory = _itemContainerResolver.ResolveInventory(_playerProvider.Player, ItemContainerId.Inventory);
            var item = inventory.Get(slot);
            if (!item.HasValue) return;

            var strategy = new InventoryToShopPreviewStrategy(
                _windowManager,
                _playerProvider,
                _itemContainerResolver,
                _itemStackFormatter,
                _shopModel,
                slot.ToInt64(),
                _sellCoefficient);

            _windowManager.InstantiateWindow<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments>(
                new ItemStacksPreviewWindowControllerArguments(
                    null,
                    slot.ToInt64(),
                    ItemContainerId.Inventory,
                    ItemContainerId.Shop,
                    ItemStacksPreviewMode.Shop,
                    strategy));
        }
    }
}
