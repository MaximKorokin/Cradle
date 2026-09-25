using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class StatsViewController : ViewControllerBase<StatsView>
    {
        private StatsViewData _statsHudData;

        public void Bind(StatsViewData statsHudData)
        {
            _statsHudData = statsHudData;
            _statsHudData.Changed += OnStatsChanged;
        }

        public void Unbind()
        {
            if (_statsHudData == null) return;

            _statsHudData.Changed -= OnStatsChanged;
            _statsHudData = null;
        }

        protected override void OnRender()
        {
            View.RequestRender(_statsHudData.Stats);
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
