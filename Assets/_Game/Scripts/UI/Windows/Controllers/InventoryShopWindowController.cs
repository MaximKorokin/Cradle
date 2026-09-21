using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Services;
using Assets._Game.Scripts.UI.Views;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class InventoryShopWindowController : WindowControllerBase<InventoryShopWindow, InventoryShopWindowControllerArguments>
    {
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

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _inventoryHudData.SetEntityId(Arguments.BuyerEntityId);
            _equipmentHudData.SetEntityId(Arguments.BuyerEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            _inventoryViewController.Initialize(Window.InventoryView);
            _inventoryViewController.Bind(_inventoryHudData);

            _shopViewController.Initialize(
                Window.ShopView,
                ShopModel,
                Arguments.ShopName,
                Arguments.BuyCoefficient,
                Arguments.SellCoefficient);
            _shopViewController.Bind();

            _inventoryViewController.SlotClick += OnInventorySlotClick;
            _shopViewController.SlotClick += OnShopSlotClick;

            Redraw();
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            _inventoryViewController.SlotClick -= OnInventorySlotClick;
            _shopViewController.SlotClick -= OnShopSlotClick;

            _inventoryViewController.Unbind();
            _shopViewController.Unbind();
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
        public IReadOnlyObservableData<string> ShopEntityId { get; }
        public IReadOnlyObservableData<string> BuyerEntityId { get; }

        public string ShopName { get; }
        public float BuyCoefficient { get; }
        public float SellCoefficient { get; }

        public ItemContainerPath ShopContainerPath => ItemContainerPath.Shop(ShopEntityId.Value);
        public ItemContainerPath InventoryContainerPath => ItemContainerPath.Inventory(BuyerEntityId.Value);
        public ItemContainerPath EquipmentContainerPath => ItemContainerPath.Equipment(BuyerEntityId.Value);

        public InventoryShopWindowControllerArguments(
            IReadOnlyObservableData<string> shopEntityId,
            IReadOnlyObservableData<string> buyerEntityId,
            string shopName,
            float buyCoefficient,
            float sellCoefficient)
        {
            ShopEntityId = shopEntityId;
            BuyerEntityId = buyerEntityId;

            ShopName = shopName;
            BuyCoefficient = buyCoefficient;
            SellCoefficient = sellCoefficient;
        }
    }
}
