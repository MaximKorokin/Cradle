using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.UI.Views;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class LocationAnnounceUISystem : UISystemBase
    {
        private IGlobalEventBus _globalEventBus;
        private LocationCatalog _locationCatalog;
        private LocationAnnounceView _locationAnnounceView;

        [Inject]
        private void Construct(
            IGlobalEventBus globalEventBus,
            LocationCatalog locationCatalog,
            LocationAnnounceView locationAnnounceView)
        {
            _globalEventBus = globalEventBus;
            _locationCatalog = locationCatalog;
            _locationAnnounceView = locationAnnounceView;

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
