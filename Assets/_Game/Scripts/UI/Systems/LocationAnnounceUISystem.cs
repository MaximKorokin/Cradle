using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.UI.Views;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class LocationAnnounceUISystem : UISystemBase
    {
        private LocationCatalog _locationCatalog;
        private LocationAnnounceView _locationAnnounceView;

        [Inject]
        private void Construct(
            IGlobalEventBus globalEventBus,
            LocationCatalog locationCatalog,
            LocationAnnounceView locationAnnounceView)
        {
            BaseConstruct(globalEventBus);

            _locationCatalog = locationCatalog;
            _locationAnnounceView = locationAnnounceView;

            TrackGlobalEvent<LocationChangedEvent>(OnLocationChanged);
        }

        private void OnLocationChanged(LocationChangedEvent e)
        {
            var locationDefinition = _locationCatalog.Get(e.LocationId);
            _locationAnnounceView.Show(locationDefinition.DisplayName);
        }
    }
}
