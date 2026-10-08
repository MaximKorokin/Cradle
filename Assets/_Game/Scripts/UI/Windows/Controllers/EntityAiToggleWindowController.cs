using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    [Window(WindowId.EntityAiToggle)]
    public sealed class EntityAiToggleWindowController : SingleViewWindowControllerBase<EntityAiToggleWindow, EntityAiToggleWindowControllerArguments, EntityAiToggleView, IEntityAiToggleViewData, EntityAiToggleViewController>
    {
        private readonly EntityAiToggleViewData _viewData;

        public EntityAiToggleWindowController(
            EntityAiToggleViewController viewController,
            EntityAiToggleViewData viewData) : base(viewController, viewData)
        {
            _viewData = viewData;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _viewData.SetEntityId(Arguments.EntityId);
        }

        protected override EntityAiToggleView GetView() => Window.EntityAiToggleView;
    }

    public readonly struct EntityAiToggleWindowControllerArguments : IWindowControllerArguments
    {
        public readonly IReadOnlyObservableData<string> EntityId;

        public EntityAiToggleWindowControllerArguments(IReadOnlyObservableData<string> entityId)
        {
            EntityId = entityId;
        }
    }
}
