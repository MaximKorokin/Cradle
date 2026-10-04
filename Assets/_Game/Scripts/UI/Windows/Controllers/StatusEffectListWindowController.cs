using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class StatusEffectListWindowController : SingleViewWindowControllerBase<StatusEffectListWindow, StatusEffectListWindowControllerArguments, StatusEffectListView, StatusEffectListViewData, StatusEffectListViewController>
    {
        public StatusEffectListWindowController(
            StatusEffectListViewController viewController,
            StatusEffectListViewData viewData) : base(viewController, viewData)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ViewData.SetEntityId(Arguments.EntityId);
        }

        protected override StatusEffectListView GetView() => Window.StatusEffectListView;
    }

    public readonly struct StatusEffectListWindowControllerArguments : IWindowControllerArguments
    {
        public readonly IReadOnlyObservableData<string> EntityId;

        public StatusEffectListWindowControllerArguments(IReadOnlyObservableData<string> entityId)
        {
            EntityId = entityId;
        }
    }
}
