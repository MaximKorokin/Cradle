using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets.CoreScripts;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public interface IAppLifecycleHandler
    {
        void OnUpdate();
        void OnPause();
        void OnFocusLost();
        void OnQuit();
    }

    public class AppLifecycleHandler : IAppLifecycleHandler
    {
        private readonly SaveConfig _saveConfig;
        private readonly SaveService _saveService;

        private readonly CooldownCounter _autosaveCooldownCounter;

        public AppLifecycleHandler(
            SaveConfig saveConfig,
            SaveService saveService)
        {
            _saveConfig = saveConfig;
            _saveService = saveService;

            _autosaveCooldownCounter = new(_saveConfig.AutosaveTime);
        }

        public void OnUpdate()
        {
            if (_autosaveCooldownCounter.TryReset())
            {
                _saveService.SaveGame();
            }
        }

        public void OnPause()
        {
            _saveService.SaveGame();
        }

        public void OnFocusLost()
        {
            _saveService.SaveGame();
        }

        public void OnQuit()
        {
            _saveService.SaveGame();
        }
    }
}
