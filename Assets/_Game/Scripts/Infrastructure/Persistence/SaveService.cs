using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;

namespace Assets._Game.Scripts.Infrastructure.Persistence
{
    public sealed class SaveService
    {
        private readonly PlayerContext _playerContext;
        private readonly EntityAssembler _entityAssembler;
        private readonly GameSaveRepository _gameSaveRepository;

        public SaveService(
            PlayerContext playerContext,
            EntityAssembler entityAssembler,
            GameSaveRepository gameSaveRepository)
        {
            _playerContext = playerContext;
            _entityAssembler = entityAssembler;
            _gameSaveRepository = gameSaveRepository;
        }

        public void SaveGame()
        {
            var save = new GameSave();
            save.PlayerSave = _entityAssembler.Save(_playerContext.Player);
            save.Version = 1;
            save.SavedAtUtc = System.DateTime.UtcNow.Ticks;
            _gameSaveRepository.Save("Player", save);
        }
    }
}
