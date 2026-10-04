using Assets._Game.Scripts.UI.Core;
using Assets._Game.Scripts.UI.Windows.Controllers;
using Assets._Game.Scripts.UI.Windows.Modal;
using Assets._Game.Scripts.Infrastructure.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Windows
{
    public class WindowManager
    {
        private readonly List<WindowStackEntry> _windowStack = new();

        private readonly RectTransform _windowsRoot;
        private readonly RectTransform _modalsRoot;
        private readonly Dictionary<WindowId, WindowDefinition> _windowDefinitions;
        private readonly Dictionary<Type, UIWindowBase> _windowPrefabsByType;
        private readonly WindowWrapper _windowWrapperPrefab;
        private readonly ModalWrapper _modalWrapperPrefab;
        private readonly IObjectResolver _resolver;

        private readonly WindowSettings _defaultWindowSettings = new(true, true, true);

        public WindowManager(
            UIRootReferences rootReferences,
            IEnumerable<WindowDefinition> windowDefinitions,
            WindowPrefabsConfig windowPrefabsConfig,
            WindowWrapper windowWrapperPrefab,
            ModalWrapper modalWrapperPrefab,
            IObjectResolver resolver)
        {
            _windowsRoot = rootReferences.WindowsRoot;
            _modalsRoot = rootReferences.ModalsRoot;

            if (windowPrefabsConfig == null || windowPrefabsConfig.WindowPrefabs == null)
            {
                throw new InvalidOperationException("Window prefab configuration is not registered or contains no prefab list.");
            }

            _windowPrefabsByType = new Dictionary<Type, UIWindowBase>();
            foreach (var prefab in windowPrefabsConfig.WindowPrefabs)
            {
                if (prefab == null)
                {
                    throw new InvalidOperationException("Window prefab configuration contains a null entry.");
                }

                var prefabType = prefab.GetType();
                if (_windowPrefabsByType.ContainsKey(prefabType))
                {
                    throw new InvalidOperationException($"More than one window prefab is registered for type {prefabType}.");
                }

                _windowPrefabsByType.Add(prefabType, prefab);
            }

            _windowDefinitions = new Dictionary<WindowId, WindowDefinition>();
            foreach (var definition in windowDefinitions)
            {
                if (definition == null)
                {
                    throw new InvalidOperationException("Window definitions contain a null entry.");
                }

                if (_windowDefinitions.ContainsKey(definition.Id))
                {
                    throw new InvalidOperationException($"More than one definition is registered for window {definition.Id}.");
                }

                _windowDefinitions.Add(definition.Id, definition);
            }
            _windowWrapperPrefab = windowWrapperPrefab;
            _modalWrapperPrefab = modalWrapperPrefab;
            _resolver = resolver;
        }

        /// <summary> Opens a window if it is not already open, otherwise closes it if it is a singleton window. If the window is not a singleton, it will open a new instance of the window. </summary>
        public void ToggleWindow(WindowId windowId, IWindowControllerArguments arguments, WindowSettings settings = default)
        {
            var definition = FindWindowDefinition(windowId);
            var existingWindow = FindWindowStackEntry(windowId, arguments);
            if (existingWindow.Window != null && definition.Configuration.IsSingleton)
            {
                CloseWindowInternal(existingWindow);
                return;
            }
            InstantiateWindow(windowId, arguments, settings);
        }

        public WindowWrapperBase OpenWindow(WindowId windowId, IWindowControllerArguments arguments, WindowSettings settings = default)
        {
            return InstantiateWindow(windowId, arguments, settings);
        }

        private WindowWrapperBase InstantiateWindow(WindowId windowId, IWindowControllerArguments arguments, WindowSettings settings = default)
        {
            // find definition
            var definition = FindWindowDefinition(windowId);

            // find existing window
            var entry = FindWindowStackEntry(windowId, arguments);
            if (definition.Configuration.IsSingleton && entry.HasData)
            {
                SLog.Warn($"Window {windowId} with arguments {arguments} is a singleton and is already open. Returning existing instance.");
                return entry.WrapperRoot;
            }

            // 1. Get controller type
            var controllerType = definition.ControllerType;

            // 2. Create window and controller
            var controller = (IWindowController)_resolver.Resolve(controllerType);
            if (!_windowPrefabsByType.TryGetValue(controller.WindowType, out var windowPrefab))
            {
                throw new InvalidOperationException($"No prefab for window type {controller.WindowType} is registered.");
            }

            var window = _resolver.Instantiate(windowPrefab, _windowsRoot);

            // 3. Initialize
            controller.Initialize(arguments);

            // 4. Bind
            controller.Bind(window);

            // 5. Wrap
            WindowWrapperBase wrapperPrefab = definition.Configuration.IsModal ? _modalWrapperPrefab : _windowWrapperPrefab;
            var wrapperParent = definition.Configuration.IsModal ? _modalsRoot : _windowsRoot;
            var wrapperRoot = _resolver.Instantiate(wrapperPrefab, wrapperParent);
            wrapperRoot.SetWindow(window);

            if (wrapperRoot is WindowWrapper windowWrapper)
            {
                var settingsToUse = settings.HasData ? settings : _defaultWindowSettings;
                windowWrapper.SetupWrapperHeader(settingsToUse.ShowHeader, settingsToUse.ShowCloseButton, settingsToUse.ShowHeaderText, windowPrefab.name);
            }

            // push window and controller to stack that will be used to destroy everything correctly
            _windowStack.Add(new(windowId, window, controller, arguments, wrapperRoot));

            // initialize window
            window.OnShow();

            return wrapperRoot;
        }

        private WindowDefinition FindWindowDefinition(WindowId windowId)
        {
            if (!_windowDefinitions.TryGetValue(windowId, out var definition))
            {
                throw new InvalidOperationException($"No definition for window {windowId} registered.");
            }

            return definition;
        }

        private WindowStackEntry FindWindowStackEntry(WindowId windowId, IWindowControllerArguments arguments)
            => _windowStack.FirstOrDefault(e => e.Id == windowId && Equals(e.Arguments, arguments));

        private void CloseWindowInternal(WindowStackEntry element)
        {
            _windowStack.Remove(element);

            element.Window.OnHide();

            if (element.WrapperRoot != null)
            {
                UnityEngine.Object.Destroy(element.WrapperRoot.gameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(element.Window.gameObject);
            }
            element.Controller.Dispose();
        }

        public void CloseWindow(UIWindowBase window)
        {
            var element = _windowStack.FirstOrDefault(e => e.Window == window);
            if (!element.HasData)
            {
                SLog.Warn($"Trying to close window {window} wih name {window.name} that is not tracked inside WinowManager");
                return;
            }

            CloseWindowInternal(element);
        }

        public void MoveWindow(WindowWrapperBase window, Vector2 delta)
        {
            window.transform.position += (Vector3)delta;
        }

        public void SetTopWindow(WindowWrapperBase window)
        {
            window.transform.SetAsLastSibling();
        }

        private readonly struct WindowStackEntry
        {
            public readonly bool HasData;

            public readonly WindowId Id;
            public readonly UIWindowBase Window;
            public readonly IWindowController Controller;
            public readonly IWindowControllerArguments Arguments;
            public readonly WindowWrapperBase WrapperRoot;

            public WindowStackEntry(
                WindowId windowId,
                UIWindowBase window,
                IWindowController controller,
                IWindowControllerArguments arguments,
                WindowWrapperBase wrapperRoot)
            {
                HasData = true;

                Id = windowId;
                Window = window;
                Controller = controller;
                Arguments = arguments;
                WrapperRoot = wrapperRoot;
            }
        }
    }

    public readonly struct WindowSettings
    {
        public readonly bool HasData;

        public readonly bool ShowHeader;
        public readonly bool ShowCloseButton;
        public readonly bool ShowHeaderText;

        public WindowSettings(bool showHeader, bool showCloseButton, bool showHeaderText)
        {
            HasData = true;

            ShowHeader = showHeader;
            ShowCloseButton = showCloseButton;
            ShowHeaderText = showHeaderText;
        }
    }
}
