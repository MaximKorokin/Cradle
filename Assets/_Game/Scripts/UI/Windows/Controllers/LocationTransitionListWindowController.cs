using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems.Location;
using Assets._Game.Scripts.Locations;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class LocationTransitionListWindowController : WindowControllerBase<LocationTransitionListWindow, EmptyWindowControllerArguments>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly LocationConfig _locationConfig;
        private readonly IPlayerProvider _playerProvider;
        private readonly WindowManager _windowManager;

        private LocationTransitionListWindow _window;

        public LocationTransitionListWindowController(
            IGlobalEventBus globalEventBus,
            LocationConfig locationConfig,
            IPlayerProvider playerProvider,
            WindowManager windowManager)
        {
            _globalEventBus = globalEventBus;
            _locationConfig = locationConfig;
            _playerProvider = playerProvider;
            _windowManager = windowManager;
        }

        public override void Bind(LocationTransitionListWindow window)
        {
            _window = window;

            _window.TransitionButtonClicked += OnTransitionButtonClicked;
            Redraw();
        }

        public override void Dispose()
        {
            _window.TransitionButtonClicked -= OnTransitionButtonClicked;
        }

        private void OnTransitionButtonClicked(LocationTransitionData transitionData)
        {
            _windowManager.CloseWindow(_window);
            _globalEventBus.Publish(new LocationTransitionRequest(transitionData.LocationDefinition.Id, transitionData.EntranceDefinition.Id));
        }

        private void Redraw()
        {
            var playerLevel = _playerProvider.Player.GetModule<LevelingModule>().Level;
            var locations = _locationConfig.GetAvailableLocations(playerLevel);
            _window.Render(locations);
        }
    }
}
