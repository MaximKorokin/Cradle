using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Systems;
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
        private readonly IGlobalEventBus _globalEventBus;
        private readonly SaveConfig _saveConfig;

        private readonly CooldownCounter _autosaveCooldownCounter;

        public AppLifecycleHandler(
            IGlobalEventBus globalEventBus,
            SaveConfig saveConfig)
        {
            _globalEventBus = globalEventBus;
            _saveConfig = saveConfig;

            _autosaveCooldownCounter = new(_saveConfig.AutosaveTime);
        }

        public void OnUpdate()
        {
            if (_autosaveCooldownCounter.TryReset())
            {
                PublishSaveGameRequest();
            }
        }

        public void OnPause()
        {
            PublishSaveGameRequest();
        }

        public void OnFocusLost()
        {
            PublishSaveGameRequest();
        }

        public void OnQuit()
        {
            PublishSaveGameRequest();
        }

        private void PublishSaveGameRequest()
        {
            _globalEventBus.Publish(new SaveGameRequest());
        }
    }
}
