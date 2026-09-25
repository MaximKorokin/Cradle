using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ShopWindowController : WindowControllerBase<ShopWindow, ShopWindowControllerArguments>
    {
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly ShopViewController _shopViewController;
        private readonly EquipmentHudData _equipmentHudData;

        private ShopModel ShopModel => _itemContainerResolver.ResolveShop(Arguments.ShopContainerPath);

        public ShopWindowController(
            ItemContainerResolver itemContainerResolver,
            ShopViewController shopViewController,
            EquipmentHudData equipmentHudData)
        {
            _itemContainerResolver = itemContainerResolver;
            _shopViewController = shopViewController;
            _equipmentHudData = equipmentHudData;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _equipmentHudData.SetEntityId(Arguments.BuyerEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            _shopViewController.InitializeShop(
                Window.ShopView,
                ShopModel,
                Arguments.ShopName,
                Arguments.BuyCoefficient,
                Arguments.SellCoefficient);
            _shopViewController.Bind();
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            _shopViewController.Unbind();
        }

        protected override void Redraw()
        {
            _shopViewController.Render();
        }

        public override void Dispose()
        {
            base.Dispose();
            _shopViewController.Dispose();
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
