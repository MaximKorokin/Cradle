using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Locations;

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
            var save = new GameSave
            {
                PlayerSave = _entityAssembler.Save(_playerContext.Player),
                Version = 1,
                SavedAtUtc = System.DateTime.UtcNow.Ticks
            };
            _gameSaveRepository.Save(SaveKey, save);
        }

        public void LoadGame()
        {
            //ResetSave();

            var gameSave = _gameSaveRepository.Load(SaveKey);

            var humanoid = _entityAssembler.Create(_newGameDefinition.PlayerEntityDefinition);
            _globalEventBus.Publish(new SpawnEntityViewRequestEvent(humanoid, UnityEngine.Vector2.zero));

            if (gameSave?.PlayerSave != null)
            {
                _entityAssembler.Apply(humanoid, gameSave.PlayerSave);
            }
            _playerContext.SetPlayer(humanoid);

            _ = _locationManager.LoadInitialLocation(_newGameDefinition.Location.Id, _newGameDefinition.LocationEntrance);
        }

        public void ResetSave()
        {
            _gameSaveRepository.Save(SaveKey, null);
        }
    }
}
