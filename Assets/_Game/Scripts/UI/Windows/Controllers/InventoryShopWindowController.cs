using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.DataFormatters;
using Assets._Game.Scripts.UI.Services;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Windows.Shared;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class InventoryShopWindowController : WindowControllerBase<InventoryShopWindow, InventoryShopWindowControllerArguments>
    {
        private InventoryShopWindow _window;

        private readonly InventoryViewController _inventoryViewController;
        private readonly ShopViewController _shopViewController;
        private readonly InventoryHudData _inventoryHudData;
        private readonly IPlayerProvider _playerProvider;
        private readonly WindowManager _windowManager;
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly ItemStackFormatter _itemStackFormatter;
        private readonly ItemPreviewService _itemPreviewService;

        private ShopItemStacksPreviewInputProcessor _previewProcessor;
        private ShopModel _shopModel;
        private InventoryModel _inventoryModel;

        public InventoryShopWindowController(
            InventoryViewController inventoryViewController,
            ShopViewController shopViewController,
            InventoryHudData inventoryHudData,
            IPlayerProvider playerProvider,
            WindowManager windowManager,
            ItemContainerResolver itemContainerResolver,
            ItemStackFormatter itemStackFormatter,
            ItemPreviewService itemPreviewService)
        {
            _inventoryViewController = inventoryViewController;
            _shopViewController = shopViewController;
            _inventoryHudData = inventoryHudData;
            _playerProvider = playerProvider;
            _windowManager = windowManager;
            _itemContainerResolver = itemContainerResolver;
            _itemStackFormatter = itemStackFormatter;
            _itemPreviewService = itemPreviewService;
        }

        public override void Initialize(InventoryShopWindowControllerArguments arguments)
        {
            base.Initialize(arguments);
            _shopModel = arguments.ShopModel;

            var player = _playerProvider.Player;
            _inventoryModel = _itemContainerResolver.ResolveInventory(player, ItemContainerId.Inventory);

            _previewProcessor = new ShopItemStacksPreviewInputProcessor(
                _itemPreviewService,
                _inventoryModel,
                _shopModel,
                arguments.BuyCoefficient,
                arguments.SellCoefficient);
        }

        public override void Bind(InventoryShopWindow window)
        {
            _window = window;
            _inventoryViewController.Initialize(_window.InventoryView);
            _inventoryViewController.Bind(_inventoryHudData);

            _shopViewController.Initialize(_window.ShopView, _shopModel);
            _shopViewController.Bind();

            _inventoryViewController.SlotClick += _previewProcessor.OnInventorySlotClick;
            _shopViewController.SlotClick += _previewProcessor.OnShopSlotClick;

            Redraw();
        }

        public override void Unbind()
        {
            _inventoryViewController.SlotClick -= _previewProcessor.OnInventorySlotClick;
            _shopViewController.SlotClick -= _previewProcessor.OnShopSlotClick;

            _inventoryViewController.Unbind();
            _shopViewController.Unbind();

            _window = null;
        }

        private void Redraw()
        {
            _inventoryViewController.Redraw();
            _shopViewController.Redraw();
        }
    }

    public sealed class InventoryShopWindowControllerArguments : IWindowControllerArguments
    {
        public ShopModel ShopModel { get; }
        public float BuyCoefficient { get; }
        public float SellCoefficient { get; }

        public InventoryShopWindowControllerArguments(ShopModel shopModel, float buyCoefficient, float sellCoefficient)
        {
            ShopModel = shopModel;
            BuyCoefficient = buyCoefficient;
            SellCoefficient = sellCoefficient;
        }
    }
}
