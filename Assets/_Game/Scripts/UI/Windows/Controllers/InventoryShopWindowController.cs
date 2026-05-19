using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Services;
using Assets._Game.Scripts.UI.Views;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class InventoryShopWindowController : WindowControllerBase<InventoryShopWindow, InventoryShopWindowControllerArguments>
    {
        private InventoryShopWindow _window;

        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly InventoryViewController _inventoryViewController;
        private readonly ShopViewController _shopViewController;
        private readonly InventoryHudData _inventoryHudData;
        private readonly EquipmentHudData _equipmentHudData;
        private readonly ItemPreviewService _itemPreviewService;

        private ShopModel ShopModel => _itemContainerResolver.ResolveShop(Arguments.ShopContainerPath);

        public InventoryShopWindowController(
            ItemContainerResolver itemContainerResolver,
            InventoryViewController inventoryViewController,
            ShopViewController shopViewController,
            InventoryHudData inventoryHudData,
            EquipmentHudData equipmentHudData,
            ItemPreviewService itemPreviewService)
        {
            _itemContainerResolver = itemContainerResolver;
            _inventoryViewController = inventoryViewController;
            _shopViewController = shopViewController;
            _inventoryHudData = inventoryHudData;
            _equipmentHudData = equipmentHudData;
            _itemPreviewService = itemPreviewService;
        }

        public override void Initialize(InventoryShopWindowControllerArguments arguments)
        {
            base.Initialize(arguments);

            _inventoryHudData.SetInventoryEntity(Arguments.InventoryContainerPath.EntityId);
            _equipmentHudData.SetEquipmentEntity(Arguments.EquipmentContainerPath.EntityId);
        }

        public override void Bind(InventoryShopWindow window)
        {
            _window = window;

            _inventoryViewController.Initialize(_window.InventoryView);
            _inventoryViewController.Bind(_inventoryHudData);

            _shopViewController.Initialize(
                _window.ShopView,
                ShopModel,
                Arguments.ShopName,
                Arguments.BuyCoefficient,
                Arguments.SellCoefficient);
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
            var item = _inventoryHudData.InventoryModel.Get(slot);
            if (item == null) return;

            var equipmentSlotToCompare = _equipmentHudData.EquipmentModel.FindOccupiedSlotForItem(item.Value);

            _itemPreviewService.ShowInventoryItemForSellPreview(
                slot.ToInt64(),
                Arguments.ShopContainerPath,
                Arguments.InventoryContainerPath,
                Arguments.EquipmentContainerPath,
                Arguments.SellCoefficient,
                equipmentSlotToCompare);
        }

        private void OnShopSlotClick(ShopSlot slot)
        {
            var item = ShopModel.Get(slot);
            if (item == null) return;

            var equipmentSlotToCompare = _equipmentHudData.EquipmentModel.FindOccupiedSlotForItem(item.Value);

            _itemPreviewService.ShowShopItemPreview(
                slot.ToInt64(),
                Arguments.ShopContainerPath,
                Arguments.InventoryContainerPath,
                Arguments.EquipmentContainerPath,
                Arguments.BuyCoefficient,
                Arguments.SellCoefficient,
                equipmentSlotToCompare);
        }

        private void Redraw()
        {
            _inventoryViewController.Redraw();
            _shopViewController.Redraw();
        }
    }

    public readonly struct InventoryShopWindowControllerArguments : IWindowControllerArguments
    {
        public ItemContainerPath ShopContainerPath { get; }
        public ItemContainerPath InventoryContainerPath { get; }
        public ItemContainerPath EquipmentContainerPath { get; }
        public string ShopName { get; }
        public float BuyCoefficient { get; }
        public float SellCoefficient { get; }

        public InventoryShopWindowControllerArguments(ItemContainerPath shopModelPath, ItemContainerPath inventoryContainerPath, ItemContainerPath equipmentContainerPath, string shopName, float buyCoefficient, float sellCoefficient)
        {
            ShopContainerPath = shopModelPath;
            InventoryContainerPath = inventoryContainerPath;
            EquipmentContainerPath = equipmentContainerPath;
            ShopName = shopName;
            BuyCoefficient = buyCoefficient;
            SellCoefficient = sellCoefficient;
        }
    }
}
