using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.Shared.Extensions;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Persistence
{
    public sealed class SaveService
    {
        private const string SaveKey = "Player";

        private readonly IGlobalEventBus _globalEventBus;
        private readonly LocationManager _locationManager;
        private readonly GameSaveRepository _gameSaveRepository;
        private readonly NewGameDefinition _newGameDefinition;
        private readonly PlayerContext _playerContext;
        private readonly EntityFactory _entityAssembler;
        private readonly InventoryModelFactory _inventoryModelAssembler;

        public SaveService(
            IGlobalEventBus globalEventBus,
            LocationManager locationManager,
            GameSaveRepository gameSaveRepository,
            NewGameDefinition newGameDefinition,
            PlayerContext playerContext,
            EntityFactory entityAssembler,
            InventoryModelFactory inventoryModelAssembler)
        {
            _globalEventBus = globalEventBus;
            _locationManager = locationManager;
            _gameSaveRepository = gameSaveRepository;
            _newGameDefinition = newGameDefinition;
            _playerContext = playerContext;
            _entityAssembler = entityAssembler;
            _inventoryModelAssembler = inventoryModelAssembler;
        }

        public void SaveGame()
        {
            var playerPosition = _playerContext.Player.GetPosition();
            var playerLocationSave = new LocationSave
            {
                LocationId = _locationManager.CurrentLocation != null ? _locationManager.CurrentLocation.Id : "",
                PositionX = playerPosition.x,
                PositionY = playerPosition.y
            };
            var save = new GameSave
            {
                PlayerSave = _entityAssembler.Save(_playerContext.Player),
                PlayerLocationSave = playerLocationSave,
                Version = 1,
                SavedAtUtc = System.DateTime.UtcNow.Ticks
            };
            _gameSaveRepository.Save(SaveKey, save);
        }

        public void LoadGame()
        {
            //ResetSave();

            var gameSave = _gameSaveRepository.Load(SaveKey);

            var playerEntity = _entityAssembler.Create(_newGameDefinition.PlayerEntityDefinition);
            _globalEventBus.Publish<SpawnEntityViewRequest>(new(playerEntity, Vector2.zero));

            // Apply player save data if it exists, otherwise the player will be in a default state.
            if (gameSave?.PlayerSave != null)
            {
                _entityAssembler.Apply(playerEntity, gameSave.PlayerSave);
            }
            _playerContext.SetPlayer(playerEntity);

            // Load the location
            if (gameSave?.PlayerLocationSave == null)
            {
                _ = _locationManager.LoadInitialLocation(_newGameDefinition.Location.Id, _newGameDefinition.LocationEntrance);
            }
            else
            {
                _ = _locationManager.LoadInitialLocation(gameSave.PlayerLocationSave.LocationId);

                var position = new Vector2(gameSave.PlayerLocationSave.PositionX, gameSave.PlayerLocationSave.PositionY);
                playerEntity.Publish<EntityPlacementRequest>(new(playerEntity, position));
            }
        }

        public void ResetSave()
        {
            _gameSaveRepository.Save(SaveKey, null);
        }
    }
}
