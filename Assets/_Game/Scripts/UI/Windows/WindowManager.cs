using Assets._Game.Scripts.UI.Core;
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
        private readonly Stack<(UIWindowBase Window, IDisposable Controller)> _windowStack = new();

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

        public UIWindowBase InstantiateWindow(WindowId windowId, params Type[] controllerTypes)
        {
            var windowType = _windowDefinitions.FirstOrDefault(d => d.Id == windowId).WindowType;
            if (windowType == null) throw new InvalidOperationException($"No definition for window of type {windowType} registered.");

            return InstantiateWindow(windowType, controllerTypes);
        }

        public T InstantiateWindow<T, K>(K controllerArguments, params Type[] controllerTypes)
            where T : UIWindowBase
            where K : IWindowControllerArguments
        {
            return (T)InstantiateWindow(typeof(T), controllerTypes, (w, c) => ((IWindowController<T, K>)c).Initialize(controllerArguments));
        }

        private UIWindowBase InstantiateWindow(Type windowType, Type[] controllerTypes, Action<UIWindowBase, object> instantiatedCallback = null)
        {
            // find prefab
            var prefab = _windowPrefabs.FirstOrDefault(w => w.GetType() == windowType);
            if (prefab == null) throw new InvalidOperationException($"No prefab for window of type {windowType} registered.");

            // find controller type
            var controllerType = _windowDefinitions.FirstOrDefault(d => d.WindowType == windowType).GetControllerType(controllerTypes);
            if (controllerType == null) throw new InvalidOperationException($"No controller for window of type {windowType} registered.");

            // create window and controller
            var controller = _resolver.Resolve(controllerType);
            var window = _resolver.Instantiate(prefab, _windowsRoot);

            // call controller Bind method
            controllerType.GetMethod("Bind").Invoke(controller, new object[] { window });

            // call callback (e.g. for controller Initialize method)
            instantiatedCallback?.Invoke(window, controller);
            
            // push window and controller to stack that will be used to destroy everything correctly
            _windowStack.Push((window, controller as IDisposable));

            // initialize window
            window.OnShow();

            // wrap window into a modal if needed
            if (prefab.IsModal)
            {
                var modalWrapper = _resolver.Instantiate(_modalWrapperPrefab, _modalsRoot);
                modalWrapper.SetWindow(window);
            }

            return window;
        }

        public void CloseTop()
        {
            if (_windowStack.Count == 0) return;
            var (window, controller) = _windowStack.Pop();
            window.OnHide();
            if (window.IsModal)
            {
                // The window is wrapped in a modal wrapper, so we need to destroy the parent game object.
                // todo: this is a bit hacky, consider refactoring the modal wrapper to avoid this.
                UnityEngine.Object.Destroy(window.transform.parent.parent.gameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(window.gameObject);
            }
            controller.Dispose();
        }
    }
}
