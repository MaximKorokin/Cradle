using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Services;
using Assets._Game.Scripts.UI.Views;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class InventoryShopWindowController : WindowControllerBase<InventoryShopWindow, InventoryShopWindowControllerArguments>
    {
        private InventoryShopWindow _window;

        private readonly InventoryViewController _inventoryViewController;
        private readonly ShopViewController _shopViewController;
        private readonly InventoryHudData _inventoryHudData;
        private readonly ItemPreviewService _itemPreviewService;

        private ShopModel _shopModel;
        private string _shopName;
        private float _buyCoefficient;
        private float _sellCoefficient;

        public InventoryShopWindowController(
            InventoryViewController inventoryViewController,
            ShopViewController shopViewController,
            InventoryHudData inventoryHudData,
            ItemPreviewService itemPreviewService)
        {
            _inventoryViewController = inventoryViewController;
            _shopViewController = shopViewController;
            _inventoryHudData = inventoryHudData;
            _itemPreviewService = itemPreviewService;
        }

        public override void Initialize(InventoryShopWindowControllerArguments arguments)
        {
            _shopModel = arguments.ShopModel;
            _shopName = arguments.ShopName;
            _buyCoefficient = arguments.BuyCoefficient;
            _sellCoefficient = arguments.SellCoefficient;
        }

        public override void Bind(InventoryShopWindow window)
        {
            _window = window;
            _inventoryViewController.Initialize(_window.InventoryView);
            _inventoryViewController.Bind(_inventoryHudData);

            _shopViewController.Initialize(_window.ShopView, _shopModel, _shopName, _buyCoefficient, _sellCoefficient);
            _shopViewController.Bind();

            _inventoryViewController.SlotClick += OnInventorySlotClick;
            _shopViewController.SlotClick += OnShopSlotClick;

            Redraw();
        }

        public override void Unbind()
        {
            _inventoryViewController.SlotClick -= OnInventorySlotClick;
            _shopViewController.SlotClick -= OnShopSlotClick;

            _inventoryViewController.Unbind();
            _shopViewController.Unbind();

            _window = null;
        }

        private void OnInventorySlotClick(InventorySlot slot)
        {
            _itemPreviewService.ShowInventoryItemForSellPreview(
                slot.ToInt64(),
                _shopModel,
                _sellCoefficient);
        }

        private void OnShopSlotClick(ShopSlot slot)
        {
            _itemPreviewService.ShowShopItemPreview(
                slot.ToInt64(),
                _shopModel,
                _buyCoefficient,
                _sellCoefficient);
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
        public string ShopName { get; }
        public float BuyCoefficient { get; }
        public float SellCoefficient { get; }

        public InventoryShopWindowControllerArguments(ShopModel shopModel, string shopName, float buyCoefficient, float sellCoefficient)
        {
            ShopModel = shopModel;
            ShopName = shopName;
            BuyCoefficient = buyCoefficient;
            SellCoefficient = sellCoefficient;
        }
    }
}
