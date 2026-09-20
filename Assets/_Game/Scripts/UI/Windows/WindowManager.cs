using Assets._Game.Scripts.UI.Core;
using Assets._Game.Scripts.UI.Windows.Controllers;
using Assets._Game.Scripts.UI.Windows.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IEnumerable<UIWindowBase> _windowPrefabs;
        private readonly IEnumerable<WindowDefinition> _windowDefinitions;
        private readonly WindowWrapper _windowWrapperPrefab;
        private readonly ModalWrapper _modalWrapperPrefab;
        private readonly IObjectResolver _resolver;

        public WindowManager(
            UIRootReferences rootReferences,
            IEnumerable<UIWindowBase> windowPrefabs,
            IEnumerable<WindowDefinition> windowDefinitions,
            WindowWrapper windowWrapperPrefab,
            ModalWrapper modalWrapperPrefab,
            IObjectResolver resolver)
        {
            _windowsRoot = rootReferences.WindowsRoot;
            _modalsRoot = rootReferences.ModalsRoot;
            _windowPrefabs = windowPrefabs;
            _windowDefinitions = windowDefinitions;
            _windowWrapperPrefab = windowWrapperPrefab;
            _modalWrapperPrefab = modalWrapperPrefab;
            _resolver = resolver;
        }

        /// <summary> Opens a window if it is not already open, otherwise closes it if it is a singleton window. If the window is not a singleton, it will open a new instance of the window. </summary>
        public void ToggleWindow(WindowId windowId, IWindowControllerArguments arguments = default)
        {
            var definition = FindWindowDefinition(windowId);
            var existingWindow = FindWindowStackEntry(windowId, arguments);
            if (existingWindow.Window != null && definition.Configuration.IsSingleton)
            {
                CloseWindowInternal(existingWindow);
                return;
            }
            InstantiateWindow(windowId, arguments);
        }

        /// <summary> Uses OpenStrategy if available, otherwise provides empty arguments to the controller </summary>
        public UIWindowBase InstantiateWindow(WindowId windowId, IWindowControllerArguments arguments = default)
        {
            // find definition
            var definition = FindWindowDefinition(windowId);

            // find existing window
            var entry = FindWindowStackEntry(windowId, arguments);
            if (definition.Configuration.IsSingleton && entry.HasData)
            {
                SLog.Warn($"Window {windowId} with arguments {arguments} is a singleton and is already open. Returning existing instance.");
                return entry.Window;
            }

            // 1. Get controller type
            var controllerType = definition.ControllerType;

            // 2. Create window and controller
            var controller = (IWindowController)_resolver.Resolve(controllerType);
            var prefab = _windowPrefabs.FirstOrDefault(w => w.GetType() == controller.WindowType);
            if (prefab == null) throw new ArgumentException($"No prefab for window of type {controller.WindowType} registered.");
            var window = _resolver.Instantiate(prefab, _windowsRoot);

            // 3. Initialize
            controller.Initialize(arguments);

            // 4. Bind
            controller.Bind(window);

            // 5. Wrap
            WindowWrapperBase wrapperPrefab = definition.Configuration.IsModal ? _modalWrapperPrefab : _windowWrapperPrefab;
            var wrapperParent = definition.Configuration.IsModal ? _modalsRoot : _windowsRoot;
            var wrapperRoot = _resolver.Instantiate(wrapperPrefab, wrapperParent);
            wrapperRoot.SetWindow(window);
            wrapperRoot.WindowCloseRequested += CloseWindow;

            // push window and controller to stack that will be used to destroy everything correctly
            _windowStack.Add(new(windowId, window, controller, arguments, wrapperRoot));

            // initialize window
            window.OnShow();

            return window;
        }

        private WindowDefinition FindWindowDefinition(WindowId windowId)
        {
            var definition = _windowDefinitions.FirstOrDefault(d => d.Id == windowId);
            if (definition == null) throw new InvalidOperationException($"No definition for window {windowId} registered.");
            return definition;
        }

        private WindowStackEntry FindWindowStackEntry(WindowId windowId, IWindowControllerArguments arguments)
        {
            var entry = _windowStack.FirstOrDefault(e => e.Id == windowId && e.Arguments.Equals(arguments));
            return entry;
        }

        private void CloseWindowInternal(WindowStackEntry element)
        {
            _windowStack.Remove(element);

            element.Window.OnHide();
            
            if (element.WrapperRoot != null)
            {
                element.WrapperRoot.WindowCloseRequested -= CloseWindow;
                UnityEngine.Object.Destroy(element.WrapperRoot.gameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(element.Window.gameObject);
            }
            element.Controller.Dispose();
        }

        public void CloseTopWindow()
        {
            if (_windowStack.Count == 0) return;

            var lastElement = _windowStack.Last();

            CloseWindowInternal(lastElement);
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
}
