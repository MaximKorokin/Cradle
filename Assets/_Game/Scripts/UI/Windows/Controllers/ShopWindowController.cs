using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ShopWindowController : WindowControllerBase<ShopWindow, ShopWindowControllerArguments>
    {
        private readonly ShopViewController _shopViewController;
        private readonly ShopViewData _shopViewData;

        public ShopWindowController(
            ShopViewController shopViewController,
            ShopViewData shopViewData)
        {
            _shopViewController = shopViewController;
            _shopViewData = shopViewData;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _shopViewData.SetEntityId(Arguments.ShopEntityId);
            _shopViewData.SetShopData(Arguments.ShopName, Arguments.BuyCoefficient, Arguments.SellCoefficient);
        }

        protected override void OnBind()
        {
            base.OnBind();

            _shopViewController.Initialize(Window.ShopView);
            _shopViewController.Bind(_shopViewData);
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
            _shopViewData.Dispose();
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
        //public ItemContainerPath InventoryContainerPath => ItemContainerPath.Inventory(BuyerEntityId.Value);
        //public ItemContainerPath EquipmentContainerPath => ItemContainerPath.Equipment(BuyerEntityId.Value);

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
