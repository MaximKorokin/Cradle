using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    [Window(WindowId.Stats)]
    public sealed class StatsWindowController : SingleViewWindowControllerBase<StatsWindow, StatsWindowControllerArguments, StatsView, IStatsViewData, StatsViewController>
    {
        public StatsWindowController(
            StatsViewController statsViewController,
            StatsViewData statsHudData) : base(statsViewController, statsHudData)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            ViewData.SetEntityId(Arguments.EntityId);
        }

        protected override StatsView GetView() => Window.StatsView;
    }

    public readonly struct StatsWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<EntryRef> EntityId { get; }
        public StatsWindowControllerArguments(IReadOnlyObservableData<EntryRef> entityId)
        {
            EntityId = entityId;
        }
    }
}
