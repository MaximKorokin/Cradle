using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    [Window(WindowId.Shop)]
    public sealed class ShopWindowController : SingleViewWindowControllerBase<ShopWindow, ShopWindowControllerArguments, ShopView, ShopViewData, ShopViewController>
    {
        public ShopWindowController(
            ShopViewController shopViewController,
            ShopViewData shopViewData) : base(shopViewController, shopViewData)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ViewData.SetEntityId(Arguments.ShopEntityId);
            ViewData.SetShopData(Arguments.ShopName, Arguments.BuyCoefficient, Arguments.SellCoefficient, Arguments.BuyerEntityId);
        }

        protected override ShopView GetView() => Window.ShopView;
    }

    public readonly struct ShopWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> ShopEntityId { get; }
        public IReadOnlyObservableData<string> BuyerEntityId { get; }

        public string ShopName { get; }
        public float BuyCoefficient { get; }
        public float SellCoefficient { get; }

        public ItemContainerPath ShopContainerPath => ItemContainerPath.Shop(ShopEntityId.Value);

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
