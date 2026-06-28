using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Infrastructure.Systems.Location;
using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.Shared.Extensions;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class PersistenceSystem : EntitySystemBase
    {
        private readonly SaveService _saveService;
        private readonly NewGameDefinition _newGameDefinition;
        private readonly PlayerContext _playerContext;
        private readonly ILocationContext _locationContext;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                requiredModules: new[] { typeof(PersistenceModule) }
            );

        public PersistenceSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository repository,
            SaveService saveService,
            NewGameDefinition newGameDefinition,
            PlayerContext playerContext,
            ILocationContext locationContext) : base(globalEventBus, repository)
        {
            _saveService = saveService;
            _newGameDefinition = newGameDefinition;
            _playerContext = playerContext;
            _locationContext = locationContext;

            TrackGlobalEvent<SaveGameRequest>(OnSaveGameRequested);
            TrackGlobalEvent<LoadGameRequest>(OnLoadGameRequested);
        }

        protected override void OnEntityAdded(Entity entity)
        {
            base.OnEntityAdded(entity);

            if (!EntityQuery.Match(entity)) return;

            _saveService.LoadEntity(entity);

            var entitySave = _saveService.GetEntitySave(entity);

            // Load player save data and transition to the correct location
            if (_playerContext.Player == entity)
            {
                LoadPlayerLocation(entity, entitySave);
            }
        }

        protected override void OnEntityRemoved(Entity entity)
        {
            base.OnEntityRemoved(entity);

            if (!EntityQuery.Match(entity)) return;

            _saveService.SaveEntity(entity);
        }

        private void OnSaveGameRequested(SaveGameRequest r)
        {
            IterateMatchingEntities(entity =>
            {
                _saveService.SaveEntity(entity);
            });

            SavePlayerLocation();

            _saveService.SaveGame(r.SaveName);
        }

        private void OnLoadGameRequested(LoadGameRequest r)
        {
            _saveService.LoadGame(r.SaveName);

            GlobalEventBus.Publish(new SpawnEntityRequest(
                _newGameDefinition.PlayerEntityDefinition,
                Vector2.zero,
                new[] { new PlayerEntitySpawnInitializer(_playerContext) }));
        }

        private void SavePlayerLocation()
        {
            // Ensure player save exists
            var playerSave = _saveService.GetEntitySave(_playerContext.Player);
            if (playerSave == null)
            {
                SLog.Warn($"No player save found for player {_playerContext.Player.Id}, cannot save game.");
                return;
            }

            // Save player position and location
            var playerPosition = _playerContext.Player.GetPosition();
            var playerLocationSave = new LocationSave
            {
                LocationId = _locationContext.CurrentLocation != null ? _locationContext.CurrentLocation.Id : "",
                EntranceId = _locationContext.CurrentEntrance != null ? _locationContext.CurrentEntrance.Id : "",
                PositionX = playerPosition.x,
                PositionY = playerPosition.y
            };

            // Save player data
            playerSave.LocationSave = playerLocationSave;
        }

        private void LoadPlayerLocation(Entity entity, EntitySave playerSave)
        {
            if (playerSave?.LocationSave == null || string.IsNullOrWhiteSpace(playerSave?.LocationSave.LocationId))
            {
                GlobalEventBus.Publish(new LocationTransitionRequest(_newGameDefinition.LocationTransitionData.LocationDefinition.Id, _newGameDefinition.LocationTransitionData.EntranceDefinition.Id));
            }
            else
            {
                // null entrance id means that player will be placed in the location based on coordinates, not on specific entrance
                GlobalEventBus.Publish(new LocationTransitionRequest(playerSave?.LocationSave.LocationId, null));

                entity.SubscribeOnce<EntityViewBoundEvent>(_ =>
                {
                    var position = new Vector2(playerSave.LocationSave.PositionX, playerSave.LocationSave.PositionY);
                    entity.Publish(new EntityRepositionRequest(position));
                });
            }
        }
    }

    public readonly struct SaveGameRequest : IGlobalEvent
    {
        public string SaveName { get; }

        public SaveGameRequest(string saveName)
        {
            SaveName = saveName;
        }
    }

    public readonly struct LoadGameRequest : IGlobalEvent
    {
        public string SaveName { get; }

        public LoadGameRequest(string saveName)
        {
            SaveName = saveName;
        }
    }
}
