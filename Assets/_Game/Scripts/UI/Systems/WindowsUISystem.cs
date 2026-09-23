using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class WindowsUISystem : UISystemBase
    {
        private WindowManager _windowManager;

        private WindowWrapperBase _currentlyMovingWindow;

        [Inject]
        public void Construct(
            IGlobalEventBus globalEventBus,
            WindowManager windowManager)
        {
            base.BaseConstruct(globalEventBus);

            _windowManager = windowManager;

            TrackGlobalEvent<PointerDownEvent>(OnPointerDown);
            TrackGlobalEvent<PointerUpEvent>(OnPointerUp);
            TrackGlobalEvent<PointerMoveEvent>(OnPointerMove);

            TrackGlobalEvent<WindowToggleRequest>(OnWindowToggleRequested);
            TrackGlobalEvent<WindowOpenRequest>(OnWindowOpenRequested);
            TrackGlobalEvent<WindowCloseRequest>(OnWindowCloseRequested);
        }

        private void OnPointerDown(PointerDownEvent e)
        {
            if (!e.Context.UnderlyingElement.TryGetComponentInParent<WindowWrapperBase>(out var windowWrapper)) return;

            _windowManager.SetTopWindow(windowWrapper);

            if (!e.Context.UnderlyingElement.TryGetComponentInParent<WindowDragHandler>(out var _)) return;

            _currentlyMovingWindow = windowWrapper;
        }

        private void OnPointerUp(PointerUpEvent e)
        {
            _currentlyMovingWindow = null;
        }

        private void OnPointerMove(PointerMoveEvent e)
        {
            if (_currentlyMovingWindow == null) return;
            _windowManager.MoveWindow(_currentlyMovingWindow, e.Context.ScreenPosition - e.Context.PreviousScreenPosition);
        }

        private void OnWindowToggleRequested(WindowToggleRequest e)
        {
            _windowManager.ToggleWindow(e.WindowId, e.Arguments);
        }

        private void OnWindowOpenRequested(WindowOpenRequest e)
        {
            _windowManager.InstantiateWindow(e.WindowId, e.Arguments);
        }

        private void OnWindowCloseRequested(WindowCloseRequest e)
        {
            _windowManager.CloseWindow(e.Window);
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

    public readonly struct WindowOpenRequest : IGlobalEvent
    {
        public readonly WindowId WindowId;
        public readonly IWindowControllerArguments Arguments;

        public WindowOpenRequest(WindowId windowId, IWindowControllerArguments arguments)
        {
            WindowId = windowId;
            Arguments = arguments;
        }
    }

    public readonly struct WindowCloseRequest : IGlobalEvent
    {
        public readonly UIWindowBase Window;

        public WindowCloseRequest(UIWindowBase window)
        {
            Window = window;
        }
    }
}
