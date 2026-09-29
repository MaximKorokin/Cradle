using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class StatsViewController : ViewControllerBase<StatsView>
    {
        private IStatsViewData _statsViewData;

        public void Bind(IStatsViewData statsViewData)
        {
            _statsViewData = statsViewData;
            _statsViewData.Changed += OnStatsChanged;
        }

        public void Unbind()
        {
            if (_statsViewData == null) return;

            _statsViewData.Changed -= OnStatsChanged;
            _statsViewData = null;
        }

        protected override void OnRender()
        {
            View.RequestRender(_statsViewData);
        }

        public override void Dispose()
        {
            Unbind();
            base.Dispose();
        }

        private void OnStatsChanged()
        {
            Render();
        }
    }
}
