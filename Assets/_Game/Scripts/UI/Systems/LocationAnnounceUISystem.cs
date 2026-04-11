using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.UI.Views;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class LocationAnnounceUISystem : UISystemBase
    {
        private IGlobalEventBus _globalEventBus;
        private LocationAnnounceView _locationAnnounceView;
        private LocationCatalog _locationCatalog;

        [Inject]
        private void Construct(
            IGlobalEventBus globalEventBus,
            LocationAnnounceView locationAnnounceView,
            LocationCatalog locationCatalog)
        {
            _globalEventBus = globalEventBus;
            _locationAnnounceView = locationAnnounceView;
            _locationCatalog = locationCatalog;

            _globalEventBus.Subscribe<LocationChangedEvent>(OnLocationChanged);
        }

        public override void Dispose()
        {
            base.Dispose();

            _globalEventBus.Unsubscribe<LocationChangedEvent>(OnLocationChanged);
        }

        private void OnLocationChanged(LocationChangedEvent e)
        {
            var locationDefinition = _locationCatalog.Get(e.LocationId);
            _locationAnnounceView.Show(locationDefinition.Name);
        }
    }
}
