using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.UI.Views;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class LocationTransitionUISystem : UISystemBase
    {
        private IGlobalEventBus _globalEventBus;
        private LocationCatalog _locationCatalog;
        private LocationTransitionView _locationTransitionView;

        [Inject]
        private void Construct(
            IGlobalEventBus globalEventBus,
            LocationCatalog locationCatalog,
            LocationTransitionView locationTransitionView)
        {
            _globalEventBus = globalEventBus;
            _locationCatalog = locationCatalog;
            _locationTransitionView = locationTransitionView;

            _globalEventBus.Subscribe<LocationTransitionViewRequest>(OnLocationTransitionViewRequested);
        }

        public override void Dispose()
        {
            base.Dispose();

            _globalEventBus.Unsubscribe<LocationTransitionViewRequest>(OnLocationTransitionViewRequested);
        }

        private void OnLocationTransitionViewRequested(LocationTransitionViewRequest request)
        {
            if (request.Show)
            {
                var locationDescriptor = _locationCatalog.Get(request.TargetLocationId);
                _locationTransitionView.Show(locationDescriptor.DisplayName, request.TargetLocationId, request.TargetEntranceId);
            }
            else
            {
                _locationTransitionView.Hide();
            }
        }
    }

    public readonly struct LocationTransitionViewRequest : IGlobalEvent
    {
        public string TargetLocationId { get; }
        public string TargetEntranceId { get; }
        public bool Show { get; }

        public LocationTransitionViewRequest(string targetLocationId, string targetEntranceId, bool show)
        {
            TargetLocationId = targetLocationId;
            TargetEntranceId = targetEntranceId;
            Show = show;
        }
    }
}
