using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ShopViewController : ViewControllerBase<ShopView>
    {
        private ShopViewData _shopViewData;

        public override void Initialize(ShopView view)
        {
            base.Initialize(view);
        }

        public void Bind(ShopViewData shopViewData)
        {
            _shopViewData = shopViewData;
            _shopViewData.Changed += OnShopChanged;
        }

        public void Unbind()
        {
            if (_shopViewData != null)
            {
                _shopViewData.Changed -= OnShopChanged;
                _shopViewData = null;
            }
        }

        protected override void OnRender()
        {
            View.RequestRender(_shopViewData);
        }

        private void OnShopChanged()
        {
            Render();
        }

        public override void Dispose()
        {
            Unbind();
            base.Dispose();
        }
    }
}
