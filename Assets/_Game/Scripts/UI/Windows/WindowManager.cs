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
        private readonly List<(UIWindowBase Window, IDisposable Controller, GameObject ModalRoot)> _windowStack = new();

        private readonly RectTransform _windowsRoot;
        private readonly RectTransform _modalsRoot;
        private readonly IEnumerable<UIWindowBase> _windowPrefabs;
        private readonly IEnumerable<WindowDefinition> _windowDefinitions;
        private readonly ModalWrapper _modalWrapperPrefab;
        private readonly IObjectResolver _resolver;

        public WindowManager(
            UIRootReferences rootReferences,
            IEnumerable<UIWindowBase> windowPrefabs,
            IEnumerable<WindowDefinition> windowDefinitions,
            ModalWrapper modalWrapperPrefab,
            IObjectResolver resolver)
        {
            _windowsRoot = rootReferences.WindowsRoot;
            _modalsRoot = rootReferences.ModalsRoot;
            _windowPrefabs = windowPrefabs;
            _windowDefinitions = windowDefinitions;
            _modalWrapperPrefab = modalWrapperPrefab;
            _resolver = resolver;
        }

        public UIWindowBase InstantiateWindow(WindowId windowId)
        {
            var windowType = _windowDefinitions.FirstOrDefault(d => d.Id == windowId).WindowType;
            if (windowType == null) throw new InvalidOperationException($"No definition for window of type {windowType} registered.");

            return InstantiateWindow(windowType);
        }

        public T InstantiateWindow<T, K>(K controllerArguments)
            where T : UIWindowBase
            where K : IWindowControllerArguments
        {
            return (T)InstantiateWindow(typeof(T), (w, c) => ((IWindowController<T, K>)c).Initialize(controllerArguments));
        }

        private UIWindowBase InstantiateWindow(Type windowType, Action<UIWindowBase, object> instantiatedCallback = null)
        {
            // find prefab
            var prefab = _windowPrefabs.FirstOrDefault(w => w.GetType() == windowType);
            if (prefab == null) throw new InvalidOperationException($"No prefab for window of type {windowType} registered.");
            if (prefab.IsSingleton && _windowStack.Any(w => w.Window.GetType() == windowType))
            {
                SLog.Warn($"Window of type {windowType} is a singleton and is already open. Returning existing instance.");
                return _windowStack.First(w => w.Window.GetType() == windowType).Window;
            }

            // find controller type
            var controllerType = _windowDefinitions.FirstOrDefault(d => d.WindowType == windowType).ControllerType
                ?? throw new InvalidOperationException($"No controller for window of type {windowType} registered.");

            // create window and controller
            var controller = _resolver.Resolve(controllerType);
            var window = _resolver.Instantiate(prefab, _windowsRoot);

            // call callback (e.g. for controller Initialize method) BEFORE Bind
            instantiatedCallback?.Invoke(window, controller);

            // call controller Bind method AFTER Initialize
            controllerType.GetMethod("Bind").Invoke(controller, new object[] { window });

            GameObject modalRoot = null;
            if (prefab.IsModal)
            {
                var modalWrapper = _resolver.Instantiate(_modalWrapperPrefab, _modalsRoot);
                modalWrapper.SetWindow(window);
                modalRoot = modalWrapper.gameObject;
            }

            // push window and controller to stack that will be used to destroy everything correctly
            _windowStack.Add((window, controller as IDisposable, modalRoot));

            // initialize window
            window.OnShow();

            return window;
        }

        private void CloseWindowInternal((UIWindowBase Window, IDisposable Controller, GameObject ModalRoot) element)
        {
            _windowStack.Remove(element);

            var (window, controller, modalRoot) = element;
            window.OnHide();
            
            if (modalRoot != null)
            {
                UnityEngine.Object.Destroy(modalRoot);
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
