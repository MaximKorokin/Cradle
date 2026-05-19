using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.Shared.Extensions;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Persistence
{
    public sealed class SaveService
    {
        private const string SaveKey = "Save_0";

        private readonly IGlobalEventBus _globalEventBus;
        private readonly ILocationContext _locationContext;
        private readonly GameSaveRepository _gameSaveRepository;
        private readonly NewGameDefinition _newGameDefinition;
        private readonly PlayerContext _playerContext;
        private readonly EntityFactory _entityFactory;

        private GameSave _currentSave;

        public SaveService(
            IGlobalEventBus globalEventBus,
            ILocationContext locationContext,
            GameSaveRepository gameSaveRepository,
            NewGameDefinition newGameDefinition,
            PlayerContext playerContext,
            EntityFactory entityFactory)
        {
            _globalEventBus = globalEventBus;
            _locationContext = locationContext;
            _gameSaveRepository = gameSaveRepository;
            _newGameDefinition = newGameDefinition;
            _playerContext = playerContext;
            _entityFactory = entityFactory;
        }

        public EntitySave GetEntitySave(Entity entity)
        {
            var persistenceKey = entity.GetModule<PersistenceModule>().PersistenceKey;

            if (_currentSave.EntitySaves.TryGetValue(persistenceKey, out var entitySave))
            {
                return entitySave;
            }
            return null;
        }

        public void SaveEntity(Entity entity)
        {
            var persistenceKey = entity.GetModule<PersistenceModule>().PersistenceKey;

            var entitySave = _entityFactory.Save(entity);
            _currentSave.EntitySaves[persistenceKey] = entitySave;

            _gameSaveRepository.Save(persistenceKey, _currentSave);
        }

        public void LoadEntity(Entity entity)
        {
            var entitySave = GetEntitySave(entity);
            if (entitySave != null)
            {
                _entityFactory.Apply(entity, entitySave);
            }
        }

        public void SaveGame()
        {
            // Ensure player save exists
            var playerSave = GetEntitySave(_playerContext.Player);
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

            // Save the game
            _currentSave.Version = 1;
            _currentSave.SavedAtUtc = System.DateTime.UtcNow.Ticks;

            _gameSaveRepository.Save(SaveKey, _currentSave);
        }

        public void LoadGame()
        {
            _currentSave = _gameSaveRepository.Load(SaveKey);
            if (_currentSave == null || _currentSave.EntitySaves == null || _currentSave.EntitySaves.Count == 0)
            {
                Debug.Log("No save found, starting new game.");
                _currentSave = new GameSave
                {
                    EntitySaves = new(),
                    Version = 1,
                    SavedAtUtc = System.DateTime.UtcNow.Ticks
                };
            }

            _globalEventBus.Publish(new SpawnEntityRequest(
                _newGameDefinition.PlayerEntityDefinition,
                Vector2.zero,
                new[] { new PlayerEntitySpawnInitializer(_playerContext) }));
        }

        public void ResetPlayerLocationSave()
        {
            var playerSave = GetEntitySave(_playerContext.Player);
            playerSave.LocationSave = null;
        }

        public void ResetPlayerLevelSave()
        {
            var playerSave = GetEntitySave(_playerContext.Player);
            playerSave.LevelingSave = null;
        }

        public void ResetSave()
        {
            _gameSaveRepository.Save(SaveKey, null);
        }
    }
}
