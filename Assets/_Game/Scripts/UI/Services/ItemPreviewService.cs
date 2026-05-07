using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.UI.DataFormatters;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers.ItemPreview;

namespace Assets._Game.Scripts.UI.Services
{
    public sealed class ItemPreviewService
    {
        private readonly WindowManager _windowManager;
        private readonly IPlayerProvider _playerProvider;
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly ItemStackFormatter _itemStackFormatter;
        private readonly ItemDefinitionFormatter _itemDefinitionFormatter;

        public ItemPreviewService(
            WindowManager windowManager,
            IPlayerProvider playerProvider,
            ItemContainerResolver itemContainerResolver,
            ItemStackFormatter itemStackFormatter,
            ItemDefinitionFormatter itemDefinitionFormatter)
        {
            _windowManager = windowManager;
            _playerProvider = playerProvider;
            _itemContainerResolver = itemContainerResolver;
            _itemStackFormatter = itemStackFormatter;
            _itemDefinitionFormatter = itemDefinitionFormatter;
        }

        /// <summary>
        /// Shows a preview window for an item stack in a container.
        /// </summary>
        public void ShowItemStackPreview(
            long containerSlot,
            ItemContainerId containerId,
            ItemContainerId secondaryContainerId = ItemContainerId.Inventory,
            EquipmentSlotKey? equipmentSlotToCompare = null)
        {
            var strategy = new ContainerItemStacksPreviewStrategy(
                _windowManager,
                _playerProvider,
                _itemContainerResolver,
                _itemStackFormatter,
                equipmentSlotToCompare,
                containerSlot,
                containerId,
                secondaryContainerId);

            _windowManager.InstantiateWindow<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments>(
                new ItemStacksPreviewWindowControllerArguments(strategy));
        }

        /// <summary>
        /// Shows a preview window for a shop item with buy action.
        /// </summary>
        public void ShowShopItemPreview(
            long shopSlot,
            ShopModel shopModel,
            float buyCoefficient,
            float sellCoefficient)
        {
            var strategy = new ShopItemStacksPreviewStrategy(
                _windowManager,
                _playerProvider,
                _itemStackFormatter,
                shopModel,
                shopSlot,
                isBuying: true,
                buyCoefficient,
                sellCoefficient);

            _windowManager.InstantiateWindow<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments>(
                new ItemStacksPreviewWindowControllerArguments(strategy));
        }

        /// <summary>
        /// Shows a preview window for an inventory item with sell action to shop.
        /// </summary>
        public void ShowInventoryItemForSellPreview(
            long inventorySlot,
            ShopModel shopModel,
            float sellCoefficient)
        {
            var strategy = new InventoryToShopPreviewStrategy(
                _windowManager,
                _playerProvider,
                _itemContainerResolver,
                _itemStackFormatter,
                shopModel,
                inventorySlot,
                sellCoefficient);

            _windowManager.InstantiateWindow<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments>(
                new ItemStacksPreviewWindowControllerArguments(strategy));
        }

        /// <summary>
        /// Shows a preview window for an item definition.
        /// </summary>
        public void ShowItemDefinitionPreview(ItemDefinition itemDefinition)
        {
            var strategy = new DefinitionItemStacksPreviewStrategy(
                itemDefinition,
                _itemDefinitionFormatter);

            _windowManager.InstantiateWindow<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments>(
                new ItemStacksPreviewWindowControllerArguments(strategy));
        }
    }
}
