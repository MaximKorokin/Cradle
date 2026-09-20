using Assets._Game.Scripts.UI.Common;
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
        private readonly List<(UIWindowBase Window, IDisposable Controller, WindowWrapperBase WrapperRoot)> _windowStack = new();

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
            var existingWindow = _windowStack.FirstOrDefault(w => w.Window.GetType() == definition.WindowType);
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
            var definition = FindWindowDefinition(windowId);

            if (definition.StrategyType != null)
                return ((PlayerWindowOpenStrategy)_resolver.Resolve(definition.StrategyType)).Open();

            return InstantiateWindow(definition.WindowType);
        }

        public T InstantiateWindow<T, K>(K controllerArguments)
            where T : UIWindowBase
            where K : IWindowControllerArguments
        {
            return (T)InstantiateWindow(typeof(T), (w, c) => ((IWindowController<T, K>)c).Initialize(controllerArguments));
        }

        private UIWindowBase InstantiateWindow(Type windowType, Action<UIWindowBase, object> instantiatedCallback = null)
        {
            // find definition
            var definition = FindWindowDefinition(windowType);

            // find prefab
            var prefab = _windowPrefabs.FirstOrDefault(w => w.GetType() == windowType);
            if (prefab == null) throw new InvalidOperationException($"No prefab for window of type {windowType} registered.");
            if (definition.Configuration.IsSingleton && _windowStack.Any(w => w.Window.GetType() == windowType))
            {
                SLog.Warn($"Window of type {windowType} is a singleton and is already open. Returning existing instance.");
                return _windowStack.First(w => w.Window.GetType() == windowType).Window;
            }

            // 1. Get controller type
            var controllerType = definition.ControllerType;

            // 2. Create window and controller
            var controller = _resolver.Resolve(controllerType);
            var window = _resolver.Instantiate(prefab, _windowsRoot);

            // 3. Initialize
            instantiatedCallback?.Invoke(window, controller);

            // 4. Bind
            controllerType.GetMethod("Bind").Invoke(controller, new object[] { window });

            // 5. Wrap
            WindowWrapperBase wrapperPrefab = definition.Configuration.IsModal ? _modalWrapperPrefab : _windowWrapperPrefab;
            var wrapperRoot = _resolver.Instantiate(wrapperPrefab, _modalsRoot);
            wrapperRoot.SetWindow(window);
            wrapperRoot.WindowCloseRequested += CloseWindow;

            // push window and controller to stack that will be used to destroy everything correctly
            _windowStack.Add((window, controller as IDisposable, wrapperRoot));

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

        private WindowDefinition FindWindowDefinition(Type windowType)
        {
            var definition = _windowDefinitions.FirstOrDefault(d => d.WindowType == windowType);
            if (definition == null) throw new InvalidOperationException($"No definition for window {windowType} registered.");
            return definition;
        }

        private void CloseWindowInternal((UIWindowBase Window, IDisposable Controller, WindowWrapperBase WrapperRoot) element)
        {
            _windowStack.Remove(element);

            var (window, controller, wrapperRoot) = element;
            window.OnHide();
            
            if (wrapperRoot != null)
            {
                wrapperRoot.WindowCloseRequested -= CloseWindow;
                UnityEngine.Object.Destroy(wrapperRoot.gameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(window.gameObject);
            }
            controller.Dispose();
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
            if (element == default)
            {
                SLog.Warn($"Trying to close window {window} wih name {window.name} that is not tracked inside WinowManager");
                return;
            }

            CloseWindowInternal(element);
        }
    }
}
