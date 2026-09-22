using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Services;
using Assets._Game.Scripts.UI.Views;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ShopWindowController : WindowControllerBase<ShopWindow, ShopWindowControllerArguments>
    {
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly ShopViewController _shopViewController;
        private readonly EquipmentHudData _equipmentHudData;
        private readonly ItemPreviewService _itemPreviewService;

        private ShopModel ShopModel => _itemContainerResolver.ResolveShop(Arguments.ShopContainerPath);

        public ShopWindowController(
            ItemContainerResolver itemContainerResolver,
            ShopViewController shopViewController,
            EquipmentHudData equipmentHudData,
            ItemPreviewService itemPreviewService)
        {
            _itemContainerResolver = itemContainerResolver;
            _shopViewController = shopViewController;
            _equipmentHudData = equipmentHudData;
            _itemPreviewService = itemPreviewService;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _equipmentHudData.SetEntityId(Arguments.BuyerEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            _shopViewController.Initialize(
                Window.ShopView,
                ShopModel,
                Arguments.ShopName,
                Arguments.BuyCoefficient,
                Arguments.SellCoefficient);
            _shopViewController.Bind();

            _shopViewController.SlotClick += OnShopSlotClick;
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            _shopViewController.SlotClick -= OnShopSlotClick;

            _shopViewController.Unbind();
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

        protected override void Redraw()
        {
            _shopViewController.Redraw();
        }
    }

    public readonly struct ShopWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> ShopEntityId { get; }
        public IReadOnlyObservableData<string> BuyerEntityId { get; }

        public string ShopName { get; }
        public float BuyCoefficient { get; }
        public float SellCoefficient { get; }

        public ItemContainerPath ShopContainerPath => ItemContainerPath.Shop(ShopEntityId.Value);
        public ItemContainerPath InventoryContainerPath => ItemContainerPath.Inventory(BuyerEntityId.Value);
        public ItemContainerPath EquipmentContainerPath => ItemContainerPath.Equipment(BuyerEntityId.Value);

        public ShopWindowControllerArguments(
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
