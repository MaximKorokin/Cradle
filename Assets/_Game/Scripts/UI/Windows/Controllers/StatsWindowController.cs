using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class StatsWindowController : SingleViewWindowControllerBase<StatsWindow, StatsWindowControllerArguments, StatsView, IStatsViewData, StatsViewController>
    {
        private readonly StatsViewData _statsHudData;

        public StatsWindowController(
            StatsViewController statsViewController,
            StatsViewData statsHudData) : base(statsViewController, statsHudData)
        {
            _statsHudData = statsHudData;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _statsHudData.SetEntityId(Arguments.EntityId);
        }

        protected override StatsView GetView() => Window.StatsView;
    }

    public readonly struct StatsWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> EntityId { get; }
        public StatsWindowControllerArguments(IReadOnlyObservableData<string> entityId)
        {
            EntityId = entityId;
        }
    }
}
