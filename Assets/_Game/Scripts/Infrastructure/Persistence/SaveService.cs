using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.Shared.Extensions;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Persistence
{
    public sealed class SaveService
    {
        private const string SaveKey = "Player";

        private readonly IGlobalEventBus _globalEventBus;
        private readonly ILocationContext _locationContext;
        private readonly GameSaveRepository _gameSaveRepository;
        private readonly NewGameDefinition _newGameDefinition;
        private readonly PlayerContext _playerContext;
        private readonly EntityFactory _entityFactory;

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

        public void SaveGame()
        {
            var playerPosition = _playerContext.Player.GetPosition();
            var playerLocationSave = new LocationSave
            {
                LocationId = _locationContext.CurrentLocation != null ? _locationContext.CurrentLocation.Id : "",
                EntranceId = _locationContext.CurrentEntrance != null ? _locationContext.CurrentEntrance.Id : "",
                PositionX = playerPosition.x,
                PositionY = playerPosition.y
            };
            var save = new GameSave
            {
                PlayerSave = _entityFactory.Save(_playerContext.Player),
                PlayerLocationSave = playerLocationSave,
                Version = 1,
                SavedAtUtc = System.DateTime.UtcNow.Ticks
            };
            _gameSaveRepository.Save(SaveKey, save);
        }

        public void LoadGame()
        {
            //ResetLevelSave();
            //ResetLocationSave();
            //ResetSave();

            var gameSave = _gameSaveRepository.Load(SaveKey);

            _globalEventBus.Publish(new SpawnEntityRequest(
                _newGameDefinition.PlayerEntityDefinition,
                Vector2.zero,
                new[] { new PlayerEntitySpawnInitializer(
                    _globalEventBus,
                    _newGameDefinition,
                    _playerContext,
                    _entityFactory,
                    gameSave)
                }));
        }

        public void ResetLocationSave()
        {
            var save = _gameSaveRepository.Load(SaveKey);
            save.PlayerLocationSave = null;
            _gameSaveRepository.Save(SaveKey, save);
        }

        public void ResetLevelSave()
        {
            var save = _gameSaveRepository.Load(SaveKey);
            save.PlayerSave.LevelingSave = null;
            _gameSaveRepository.Save(SaveKey, save);
        }

        public void ResetSave()
        {
            _gameSaveRepository.Save(SaveKey, null);
        }
    }
}
