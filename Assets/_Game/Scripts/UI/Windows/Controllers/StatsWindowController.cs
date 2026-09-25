using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class StatsWindowController : WindowControllerBase<StatsWindow, StatsWindowControllerArguments>
    {
        private readonly StatsHudData _statsHudData;
        private readonly StatsViewController _statsViewController;

        public StatsWindowController(
            StatsHudData statsHudData,
            StatsViewController statsViewController)
        {
            _statsHudData = statsHudData;
            _statsViewController = statsViewController;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _statsHudData.SetEntityId(Arguments.EntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            _statsViewController.Initialize(Window.StatsView);
            _statsViewController.Bind(_statsHudData);
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            _statsViewController.Unbind();
        }

        protected override void Redraw()
        {
            _statsViewController.Render();
        }

        public override void Dispose()
        {
            base.Dispose();
            _statsViewController.Dispose();
            _statsHudData.Dispose();
        }
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
