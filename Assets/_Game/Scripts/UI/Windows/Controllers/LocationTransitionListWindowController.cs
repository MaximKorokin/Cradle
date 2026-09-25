using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems.Location;
using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.Systems;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class LocationTransitionListWindowController : WindowControllerBase<LocationTransitionListWindow, LocationTransitionListWindowControllerArguments>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly LocationConfig _locationConfig;
        private readonly EntityRepository _entityRepository;
        private readonly LocationTransitionListViewController _viewController;

        public LocationTransitionListWindowController(
            IGlobalEventBus globalEventBus,
            LocationConfig locationConfig,
            EntityRepository entityRepository,
            LocationTransitionListViewController viewController)
        {
            _globalEventBus = globalEventBus;
            _locationConfig = locationConfig;
            _entityRepository = entityRepository;
            _viewController = viewController;
        }

        protected override void OnBind()
        {
            base.OnBind();

            Window.TransitionButtonClicked += OnTransitionButtonClicked;
            _viewController.Initialize(Window.View);
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            Window.TransitionButtonClicked -= OnTransitionButtonClicked;
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
            _viewController.SetTransitions(locations);
        }

        public override void Dispose()
        {
            base.Dispose();
            _viewController.Dispose();
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
