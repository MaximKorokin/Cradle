using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class CompactEntityStatusWindowController : SingleViewWindowControllerBase<CompactEntityStatusWindow, CompactEntityStatusWindowControllerArguments, CompactEntityStatusView, EntityStatusViewData, CompactEntityStatusViewController>
    {
        public CompactEntityStatusWindowController(
            CompactEntityStatusViewController viewController,
            EntityStatusViewData viewData) : base(viewController, viewData)
        {

        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ViewData.SetEntityId(Arguments.EntityId);
        }

        protected override CompactEntityStatusView GetView() => Window.CompactPlayerStateView;
    }

    public readonly struct CompactEntityStatusWindowControllerArguments : IWindowControllerArguments
    {
        public readonly IReadOnlyObservableData<string> EntityId;

        public CompactEntityStatusWindowControllerArguments(IReadOnlyObservableData<string> entityId)
        {
            EntityId = entityId;
        }
    }
}
