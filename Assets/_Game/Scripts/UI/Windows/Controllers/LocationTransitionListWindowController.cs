using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems.Location;
using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Systems;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class LocationTransitionListWindowController : WindowControllerBase<LocationTransitionListWindow, LocationTransitionListWindowControllerArguments>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly LocationConfig _locationConfig;
        private readonly EntityRepository _entityRepository;
        private readonly LocationTransitionListViewController _viewController;
        private readonly LocationTransitionListViewData _viewData;

        public LocationTransitionListWindowController(
            IGlobalEventBus globalEventBus,
            LocationConfig locationConfig,
            EntityRepository entityRepository,
            LocationTransitionListViewController viewController,
            LocationTransitionListViewData viewData)
        {
            _globalEventBus = globalEventBus;
            _locationConfig = locationConfig;
            _entityRepository = entityRepository;
            _viewController = viewController;
            _viewData = viewData;
        }

        protected override void OnBind()
        {
            base.OnBind();

            Window.LocationTransitionListView.TransitionButtonClicked += OnTransitionButtonClicked;
            _viewController.Initialize(Window.LocationTransitionListView);
            _viewController.Bind(_viewData);
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            Window.LocationTransitionListView.TransitionButtonClicked -= OnTransitionButtonClicked;
            _viewController.Unbind();
        }

        private void OnTransitionButtonClicked(LocationTransitionData transitionData)
        {
            _globalEventBus.Publish(new WindowCloseRequest(Window));
            _globalEventBus.Publish(new LocationTransitionRequest(transitionData.LocationDefinition.Id, transitionData.EntranceDefinition.Id));
        }

        protected override void Redraw()
        {
            var playerLevel = _entityRepository.Get(Arguments.EntityId.Value).GetModule<LevelingModule>().Level;
            var locations = _locationConfig.GetAvailableLocations(playerLevel);
            _viewData.SetTransitions(locations);
            _viewController.Render();
        }

        public override void Dispose()
        {
            base.Dispose();
            _viewController.Dispose();
            _viewData.Dispose();
        }
    }

    public readonly struct LocationTransitionListWindowControllerArguments : IWindowControllerArguments
    {
        public readonly IReadOnlyObservableData<string> EntityId;

        public LocationTransitionListWindowControllerArguments(IReadOnlyObservableData<string> entityId)
        {
            EntityId = entityId;
        }
    }
}
