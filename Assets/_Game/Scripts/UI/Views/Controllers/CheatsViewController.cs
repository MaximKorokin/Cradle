using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class CheatsViewController : ViewControllerBase<CheatsView>
    {
        private CheatsViewData _cheatsViewData;

        public void Bind(CheatsViewData cheatsViewData)
        {
            _cheatsViewData = cheatsViewData;
            _cheatsViewData.Changed += OnCheatsDataChanged;
        }

        public void Unbind()
        {
            if (_cheatsViewData == null) return;

            _cheatsViewData.Changed -= OnCheatsDataChanged;
            _cheatsViewData = null;
        }

        protected override void OnRender()
        {
            View.RequestRender(_cheatsViewData);
        }

        public override void Dispose()
        {
            Unbind();
            base.Dispose();
        }

        private void OnCheatsDataChanged()
        {
            Render();
        }
    }
}
