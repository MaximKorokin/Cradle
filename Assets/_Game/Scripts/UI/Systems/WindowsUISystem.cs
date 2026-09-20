using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class WindowsUISystem : UISystemBase
    {
        private WindowManager _windowManager;

        [Inject]
        public void Construct(
            IGlobalEventBus globalEventBus,
            WindowManager windowManager)
        {
            base.BaseConstruct(globalEventBus);

            _windowManager = windowManager;

            TrackGlobalEvent<WindowToggleRequest>(OnWindowToggleRequested);
        }

        private void OnWindowToggleRequested(WindowToggleRequest e)
        {
            _windowManager.ToggleWindow(e.WindowId, e.Arguments);
        }
    }

    public readonly struct WindowToggleRequest : IGlobalEvent
    {
        public readonly WindowId WindowId;
        public readonly IWindowControllerArguments Arguments;

        public WindowToggleRequest(WindowId windowId, IWindowControllerArguments arguments)
        {
            WindowId = windowId;
            Arguments = arguments;
        }
    }
}
