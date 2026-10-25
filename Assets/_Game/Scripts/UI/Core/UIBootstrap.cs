using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public sealed class UIBootstrap : IStartable
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly WindowsInitializerService _windowsInitializerService;
        
        public UIBootstrap(
            IGlobalEventBus globalEventBus,
            WindowsInitializerService windowsInitializerService)
        {
            _globalEventBus = globalEventBus;
            _windowsInitializerService = windowsInitializerService;

            _globalEventBus.SubscribeOnce<GameLoadedEvent>(OnGameLoaded);
        }

        public void Start()
        {
            _globalEventBus.Publish(new UIInitializedEvent());
        }

        private void OnGameLoaded(GameLoadedEvent e)
        {
            _windowsInitializerService.InitializeWindows();
        }
    }

    public readonly struct UIInitializedEvent : IGlobalEvent
    {
    }
}
