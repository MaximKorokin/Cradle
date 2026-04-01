using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Locations;
using System;
using System.Threading.Tasks;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class LocationSystem : SystemBase
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly LocationManager _locationManager;

        public LocationSystem(IGlobalEventBus globalEventBus, LocationManager locationManager)
        {
            _globalEventBus = globalEventBus;
            _locationManager = locationManager;

            _globalEventBus.Subscribe<LocationTransitionRequest>(OnLocationTransitionRequested);
        }

        public override void Dispose()
        {
            base.Dispose();

            _globalEventBus.Unsubscribe<LocationTransitionRequest>(OnLocationTransitionRequested);
        }

        private void OnLocationTransitionRequested(LocationTransitionRequest request)
        {
            _ = HandleTransitionAsync(request);
        }

        private async Task HandleTransitionAsync(LocationTransitionRequest request)
        {
            try
            {
                if (_locationManager.CurrentLocation == null)
                    await _locationManager.LoadInitialLocation(request.LocationId, request.EntranceId);
                else
                    await _locationManager.TransitToLocation(request.LocationId, request.EntranceId);
            }
            catch (Exception e)
            {
                SLog.Error($"Failed to transition to location '{request.LocationId}' via entrance '{request.EntranceId}': {e}");
            }
        }
    }

    public readonly struct LocationTransitionRequest : IGlobalEvent
    {
        public readonly string LocationId;
        public readonly string EntranceId;

        public LocationTransitionRequest(string locationId, string entranceId)
        {
            LocationId = locationId;
            EntranceId = entranceId;
        }
    }
}
