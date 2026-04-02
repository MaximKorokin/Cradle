using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Locations.Markers;
using Assets._Game.Scripts.Shared.Extensions;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Assets._Game.Scripts.Locations
{
    public interface ILocationContext
    {
        LocationDefinition CurrentLocation { get; }
        bool IsTransitionInProgress { get; }
    }

    public interface ILocationService
    {
        Task LoadInitialLocation(string locationId);
        Task LoadInitialLocation(string locationId, string entranceId);
        Task TransitToLocation(string locationId, string entranceId);
    }

    public sealed class LocationManager : ILocationContext, ILocationService
    {
        private readonly IPlayerProvider _playerProvider;
        private readonly LocationCatalog _locationCatalog;
        private readonly IGlobalEventBus _globalEventBus;

        private Scene _currentScene;
        private string _currentLocationId;

        public LocationDefinition CurrentLocation { get; private set; }
        public bool IsTransitionInProgress { get; private set; }

        public LocationManager(
            IPlayerProvider playerProvider,
            LocationCatalog locationCatalog,
            IGlobalEventBus globalEventBus)
        {
            _playerProvider = playerProvider;
            _locationCatalog = locationCatalog;
            _globalEventBus = globalEventBus;
        }

        public async Task LoadInitialLocation(string locationId)
        {
            await LoadInitialLocation(locationId, entranceId: null);
        }

        public async Task LoadInitialLocation(string locationId, string entranceId)
        {
            if (CurrentLocation != null)
                throw new InvalidOperationException("Initial location is already loaded.");

            await LoadLocationInternal(locationId, entranceId, unloadCurrent: false);
        }

        public async Task TransitToLocation(string locationId, string entranceId)
        {
            if (IsTransitionInProgress)
                return;

            await LoadLocationInternal(locationId, entranceId, unloadCurrent: true);
        }

        private async Task LoadLocationInternal(string locationId, string entranceId, bool unloadCurrent)
        {
            IsTransitionInProgress = true;

            try
            {
                var nextLocation = _locationCatalog.Get(locationId);

                _globalEventBus.Publish(new LocationChangingEvent(_currentLocationId, locationId));

                if (unloadCurrent && _currentScene.IsValid() && _currentScene.isLoaded)
                    await SceneManager.UnloadSceneAsync(_currentScene).AsTask();

                var loadOperation = SceneManager.LoadSceneAsync(nextLocation.SceneName, LoadSceneMode.Additive);
                await loadOperation.AsTask();

                var scene = SceneManager.GetSceneByName(nextLocation.SceneName);
                if (!scene.IsValid() || !scene.isLoaded)
                    throw new InvalidOperationException($"Scene '{nextLocation.SceneName}' was not loaded.");

                _currentScene = scene;
                _currentLocationId = nextLocation.Id;
                CurrentLocation = nextLocation;

                if (!string.IsNullOrWhiteSpace(entranceId))
                {
                    var entrance = FindEntrance(scene, entranceId);
                    MovePlayerToEntrance(entrance);
                }

                _globalEventBus.Publish(new LocationChangedEvent(locationId, entranceId));
            }
            catch (Exception ex)
            {
                SLog.Error($"Failed to load location '{locationId}': {ex}");
            }
            finally
            {
                IsTransitionInProgress = false;
            }
        }

        private static LocationEntranceMarker FindEntrance(Scene scene, string entranceId)
        {
            var roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                var entrances = roots[i].GetComponentsInChildren<LocationEntranceMarker>(true);
                for (int j = 0; j < entrances.Length; j++)
                {
                    if (entrances[j].EntranceId == entranceId)
                        return entrances[j];
                }
            }

            throw new InvalidOperationException(
                $"Entrance '{entranceId}' was not found in scene '{scene.name}'.");
        }

        private void MovePlayerToEntrance(LocationEntranceMarker entrance)
        {
            var playerEntity = _playerProvider.Player;

            playerEntity.Publish(new EntityRepositionRequest(playerEntity, entrance.Position));
        }
    }

    public readonly struct LocationChangingEvent : IGlobalEvent
    {
        public readonly string FromLocationId;
        public readonly string ToLocationId;

        public LocationChangingEvent(string fromLocationId, string toLocationId)
        {
            FromLocationId = fromLocationId;
            ToLocationId = toLocationId;
        }
    }

    public readonly struct LocationChangedEvent : IGlobalEvent
    {
        public readonly string LocationId;
        public readonly string EntranceId;

        public LocationChangedEvent(string locationId, string entranceId)
        {
            LocationId = locationId;
            EntranceId = entranceId;
        }
    }
}