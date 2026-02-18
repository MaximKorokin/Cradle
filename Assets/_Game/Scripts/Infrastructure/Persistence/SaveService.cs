using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items.Inventory;

namespace Assets._Game.Scripts.Infrastructure.Persistence
{
    public sealed class SaveService
    {
        private const string SaveKey = "Player";

        private readonly GameSaveRepository _gameSaveRepository;
        private readonly NewGameDefinition _newGameDefinition;
        private readonly PlayerContext _playerContext;
        private readonly EntityAssembler _entityAssembler;
        private readonly InventoryModelAssembler _inventoryModelAssembler;

        public SaveService(
            GameSaveRepository gameSaveRepository,
            NewGameDefinition newGameDefinition,
            PlayerContext playerContext,
            EntityAssembler entityAssembler,
            InventoryModelAssembler inventoryModelAssembler)
        {
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
                StashSave = _inventoryModelAssembler.Save(_playerContext.StashInventory),
                Version = 1,
                SavedAtUtc = System.DateTime.UtcNow.Ticks
            };
            _gameSaveRepository.Save(SaveKey, save);
        }

        public void LoadGame()
        {
            //ResetSave();
            var gameSave = _gameSaveRepository.Load(SaveKey);

            // hardcoded 100 slots in stash
            var stash = _inventoryModelAssembler.Create(100);
            if (gameSave?.StashSave != null) _inventoryModelAssembler.Apply(stash, gameSave.StashSave);
            _playerContext.SetStash(stash);

            Entity humanoid;
            if (gameSave?.PlayerSave != null)
            {
                humanoid = _entityAssembler.Assemble(_newGameDefinition.PlayerEntityDefinition, gameSave.PlayerSave);
            }
            else
            {
                humanoid = _entityAssembler.Create(_newGameDefinition.PlayerEntityDefinition);
            }
            _playerContext.SetPlayer(humanoid);
        }

        public void ResetSave()
        {
            _gameSaveRepository.Save(SaveKey, null);
        }
    }
}
