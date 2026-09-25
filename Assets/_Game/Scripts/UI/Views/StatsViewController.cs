using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class StatsViewController : ViewControllerBase<StatsView>
    {
        private StatsHudData _statsHudData;

        public void Bind(StatsHudData statsHudData)
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
