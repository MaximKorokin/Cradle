using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.UI.Windows;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Persistence
{
    public interface ISaveService
    {
        EntitySave GetEntitySave(Entity entity);
        void SaveGame(string saveName);
        void LoadGame(string saveName);
        void ResetSave();
    }

    public interface IEntitySaveService
    {
        void SaveEntity(Entity entity);
        void LoadEntity(Entity entity);
    }

    public interface IWindowsSaveService
    {
        void SaveWindow(WindowId windowId, Vector2 position);
        Vector2? LoadWindow(WindowId windowId);
        void ResetWindows();
    }

    public sealed class SaveService : ISaveService, IEntitySaveService, IWindowsSaveService
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

        public void SaveWindow(WindowId windowId, Vector2 position)
        {
            _currentSave.WindowSaves ??= new();
            _currentSave.WindowSaves[windowId] = new WindowSave
            {
                WindowId = windowId,
                Position = position
            };
        }

        public Vector2? LoadWindow(WindowId windowId)
        {
            if (_currentSave.WindowSaves != null && _currentSave.WindowSaves.TryGetValue(windowId, out var windowSave))
            {
                return windowSave.Position;
            }
            return null;
        }

        public void ResetWindows()
        {
            _currentSave.WindowSaves = new();
        }
    }
}
