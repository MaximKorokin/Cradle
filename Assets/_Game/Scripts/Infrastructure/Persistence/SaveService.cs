using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;

namespace Assets._Game.Scripts.Infrastructure.Persistence
{
    public sealed class SaveService
    {
        private const string DefaultSaveKey = "Save_0";

        private readonly GameSaveRepository _gameSaveRepository;
        private readonly EntityFactory _entityFactory;

        private GameSave _currentSave;

        public SaveService(
            GameSaveRepository gameSaveRepository,
            EntityFactory entityFactory)
        {
            _gameSaveRepository = gameSaveRepository;
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

        public void SaveGame(string saveName)
        {
            if (string.IsNullOrWhiteSpace(saveName)) saveName = DefaultSaveKey;

            _currentSave.Version = 1;
            _currentSave.SavedAtUtc = System.DateTime.UtcNow.Ticks;

            _gameSaveRepository.Save(saveName, _currentSave);
        }

        public void LoadGame(string saveName)
        {
            if (string.IsNullOrWhiteSpace(saveName)) saveName = DefaultSaveKey;

            _currentSave = _gameSaveRepository.Load(saveName);
            if (_currentSave == null || _currentSave.EntitySaves == null || _currentSave.EntitySaves.Count == 0)
            {
                SLog.Info("No save found, starting new game.");
                _currentSave = new GameSave
                {
                    EntitySaves = new(),
                    Version = 1,
                    SavedAtUtc = System.DateTime.UtcNow.Ticks
                };
            }
        }

        public void ResetSave()
        {
            _gameSaveRepository.Save(DefaultSaveKey, null);
        }
    }
}
