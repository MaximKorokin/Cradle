using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Locations.Markers;
using System;
using System.Linq;
using VContainer.Unity;

namespace Assets._Game.Scripts.Locations.Core
{
    public sealed class LocationBootstrap : IStartable
    {
        private readonly IPlayerProvider _playerProvider;
        private readonly ILocationContext _locationContext;
        private readonly LocationBordersContext _locationBordersContext;
        private readonly LocationMarkersContext _locationMarkersContext;
        private readonly ICameraService _cameraService;

        public LocationBootstrap(
            IPlayerProvider playerProvider,
            ILocationContext locationContext,
            LocationBordersContext locationBordersContext,
            LocationMarkersContext locationMarkersContext,
            ICameraService cameraService)
        {
            _playerProvider = playerProvider;
            _locationContext = locationContext;
            _locationBordersContext = locationBordersContext;
            _locationMarkersContext = locationMarkersContext;
            _cameraService = cameraService;
        }

        public void Start()
        {
            _cameraService.SetConfiner(_locationBordersContext.CameraBorder);

            if (!string.IsNullOrWhiteSpace(_locationContext.CurrentEntranceId))
            {
                var entrance = FindEntrance(_locationContext.CurrentEntranceId);
                MovePlayerToEntrance(entrance);
            }
        }

        private LocationEntranceMarker FindEntrance(string entranceId)
        {
            if (_locationMarkersContext == null)
                throw new InvalidOperationException($"Scene '{_locationContext.CurrentLocation.name}' does not contain a {nameof(LocationMarkersContext)}.");

            var entrance = _locationMarkersContext.LocationEntranceMarkers.FirstOrDefault(e => e.EntranceId == entranceId);

            if (entrance == null)
                throw new InvalidOperationException($"Entrance '{entranceId}' was not found in scene '{_locationContext.CurrentLocation.name}'.");

            return entrance;
        }

        private void MovePlayerToEntrance(LocationEntranceMarker entrance)
        {
            var playerEntity = _playerProvider.Player;

            playerEntity.Publish(new EntityRepositionRequest(entrance.Position));
        }
    }
}
