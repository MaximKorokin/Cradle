using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class WindowsUISystem : UISystemBase
    {
        private readonly Vector2 _defaultWindowPosition = new(Screen.width / 2, Screen.height / 2);
        private IReadOnlyDictionary<WindowId, Vector2> _windowsInitialData;

        private IWindowsSaveService _windowsSaveService;
        private WindowManager _windowManager;

        private WindowWrapperBase _currentlyMovingWindow;
        private Vector2 _previousWindowMovePointerPosition;

        [Inject]
        public void Construct(
            IGlobalEventBus globalEventBus,
            IWindowsSaveService windowsSaveService,
            InitialWindowsDataProvider initialWindowsDataProvider,
            WindowManager windowManager)
        {
            BaseConstruct(globalEventBus);

            _windowsInitialData = initialWindowsDataProvider.GetWindowsInitialData().ToDictionary(x => x.WindowId, x => x.Position);

            _windowsSaveService = windowsSaveService;
            _windowManager = windowManager;

            TrackGlobalEvent<PointerDownEvent>(OnPointerDown);
            TrackGlobalEvent<PointerUpEvent>(OnPointerUp);
            TrackGlobalEvent<PointerMoveEvent>(OnPointerMove);

            TrackGlobalEvent<WindowToggleRequest>(OnWindowToggleRequested);
            TrackGlobalEvent<WindowOpenRequest>(OnWindowOpenRequested);
            TrackGlobalEvent<WindowCloseRequest>(OnWindowCloseRequested);
            TrackGlobalEvent<WindowPositionsResetRequest>(OnWindowPositionsResetRequested);
        }

        private void OnPointerDown(PointerDownEvent e)
        {
            if (!e.Context.UnderlyingElement.TryGetComponentInParent<WindowWrapperBase>(out var windowWrapper)) return;

            _windowManager.SetTopWindow(windowWrapper);

            if (!e.Context.UnderlyingElement.TryGetComponentInParent<WindowDragHandler>(out var _)) return;

            _currentlyMovingWindow = windowWrapper;
            _previousWindowMovePointerPosition = e.Context.ScreenPosition;
        }

        private void OnPointerUp(PointerUpEvent e)
        {
            _currentlyMovingWindow = null;
        }

        private void OnPointerMove(PointerMoveEvent e)
        {
            if (_currentlyMovingWindow == null) return;
            var pointerDelta = e.Context.ScreenPosition - _previousWindowMovePointerPosition;
            _previousWindowMovePointerPosition = e.Context.ScreenPosition;
            _windowManager.MoveWindow(_currentlyMovingWindow, pointerDelta);
            _windowsSaveService.SaveWindow(_currentlyMovingWindow.WindowId, _currentlyMovingWindow.transform.position);
        }

        private void OnWindowToggleRequested(WindowToggleRequest e)
        {
            var windowWrapper = _windowManager.ToggleWindow(e.WindowId, e.Arguments, e.Settings);
            if (windowWrapper != null)
                SetWindowPosition(windowWrapper, e.Position);
        }

        private void OnWindowOpenRequested(WindowOpenRequest e)
        {
            var windowWrapper = _windowManager.OpenWindow(e.WindowId, e.Arguments, e.Settings);
            if (windowWrapper != null)
                SetWindowPosition(windowWrapper, e.Position);
            e.Callback?.Invoke(windowWrapper);
        }

        private void OnWindowCloseRequested(WindowCloseRequest e)
        {
            _windowManager.CloseWindow(e.Window);
        }

        private void OnWindowPositionsResetRequested(WindowPositionsResetRequest e)
        {
            foreach (var window in _windowManager.ActiveWindows)
            {
                if (!_windowsInitialData.TryGetValue(window.WindowId, out var position))
                {
                    position = _defaultWindowPosition;
                }
                _windowManager.SetWindowPosition(window, position);
            }
            _windowsSaveService.ResetWindows();
        }

        private void SetWindowPosition(WindowWrapperBase windowWrapper, Vector2? position)
        {
            var loadedPosition = _windowsSaveService.LoadWindow(windowWrapper.WindowId);
            if (windowWrapper != null)
            {
                if (loadedPosition.HasValue)
                {
                    _windowManager.SetWindowPosition(windowWrapper, loadedPosition.Value);
                }
                else if (position.HasValue)
                {
                    _windowManager.SetWindowPosition(windowWrapper, position.Value);
                }
            }
        }
    }

    public readonly struct WindowToggleRequest : IGlobalEvent
    {
        public readonly WindowId WindowId;
        public readonly IWindowControllerArguments Arguments;
        public readonly WindowSettings Settings;
        public readonly Vector2? Position;

        public WindowToggleRequest(WindowId windowId, IWindowControllerArguments arguments, WindowSettings settings = default, Vector2? position = null)
        {
            WindowId = windowId;
            Arguments = arguments;
            Settings = settings;
            Position = position;
        }
    }

    public readonly struct WindowOpenRequest : IGlobalEvent
    {
        public readonly WindowId WindowId;
        public readonly IWindowControllerArguments Arguments;
        public readonly WindowSettings Settings;
        public readonly Vector2? Position;
        public readonly Action<WindowWrapperBase> Callback;

        public WindowOpenRequest(WindowId windowId, IWindowControllerArguments arguments, WindowSettings settings = default, Vector2? position = null, Action<WindowWrapperBase> callback = null)
        {
            WindowId = windowId;
            Arguments = arguments;
            Settings = settings;
            Position = position;
            Callback = callback;
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

    public readonly struct WindowPositionsResetRequest : IGlobalEvent
    {
    }
}
