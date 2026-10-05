using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Systems;
using Assets._Game.Scripts.UI.Windows;

namespace Assets._Game.Scripts.UI.Core
{
    public sealed class WindowsInitializerService
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly InitialWindowsDataProvider _initialWindowsDataProvider;
        private readonly WindowControllerArgumentsProvider _windowControllerArgumentsProvider;

        public WindowsInitializerService(
            IGlobalEventBus globalEventBus,
            InitialWindowsDataProvider initialWindowsDataProvider,
            WindowControllerArgumentsProvider windowControllerArgumentsProvider)
        {
            _globalEventBus = globalEventBus;
            _initialWindowsDataProvider = initialWindowsDataProvider;
            _windowControllerArgumentsProvider = windowControllerArgumentsProvider;
        }

        public void InitializeWindows()
        {
            foreach (var data in _initialWindowsDataProvider.GetWindowsInitialData())
            {
                var arguments = _windowControllerArgumentsProvider.GetPlayerArguments(data.WindowId);
                _globalEventBus.Publish(new WindowOpenRequest(data.WindowId, arguments, data.WindowSettings, data.Position));
            }
        }
    }
}
