using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class CraftingViewController : ViewControllerBase<CraftingView>
    {
        private CraftingViewData _craftingViewData;

        public void Bind(CraftingViewData craftingViewData)
        {
            _craftingViewData = craftingViewData;
            _craftingViewData.Changed += OnCraftingDataChanged;
        }

        public void Unbind()
        {
            if (_craftingViewData == null) return;

            _craftingViewData.Changed -= OnCraftingDataChanged;
            _craftingViewData = null;
        }

        protected override void OnRender()
        {
            View.RequestRender(_craftingViewData);
        }

        public override void Dispose()
        {
            Unbind();
            base.Dispose();
        }

        private void OnCraftingDataChanged()
        {
            Render();
        }
    }
}
