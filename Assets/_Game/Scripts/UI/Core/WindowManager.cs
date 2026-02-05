using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.Core;
using Assets._Game.Scripts.UI.Windows.Modal;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Windows
{
    public class WindowManager
    {
        private readonly Stack<(UIWindow Window, IDisposable Controller)> _windowStack = new();

        private readonly RectTransform _windowsRoot;
        private readonly RectTransform _modalsRoot;
        private readonly IEnumerable<UIWindow> _windowPrefabs;
        private readonly ModalWrapper _modalWrapperPrefab;
        private readonly IObjectResolver _resolver;

        public WindowManager(
            UIRootReferences rootReferences,
            IEnumerable<UIWindow> windowPrefabs,
            ModalWrapper modalWrapperPrefab,
            IObjectResolver resolver)
        {
            _windowsRoot = rootReferences.WindowsRoot;
            _modalsRoot = rootReferences.ModalsRoot;
            _windowPrefabs = windowPrefabs;
            _modalWrapperPrefab = modalWrapperPrefab;
            _resolver = resolver;
        }

        private T InstantiateWindow<T>() where T : UIWindow
        {
            var prefab = System.Linq.Enumerable.FirstOrDefault(_windowPrefabs, w => w is T);
            if (prefab == null) throw new InvalidOperationException($"No prefab for window of type {typeof(T)} registered.");
            var window = _resolver.Instantiate((T)prefab, _windowsRoot);
            if (prefab.IsModal)
            {
                var modalWrapper = _resolver.Instantiate(_modalWrapperPrefab, _modalsRoot);
                modalWrapper.SetWindow(window);
            }
            return window;
        }

        public InventoryEquipmentWindow ShowInventoryEquipmentWindow(InventoryModel inventoryModel, EquipmentModel equipmentModel, ItemCommandHandler handler)
        {
            var window = InstantiateWindow<InventoryEquipmentWindow>();
            var controller = new InventoryEquipmentWindowController(this, window, inventoryModel, equipmentModel, handler);
            PushToStack(window, controller);
            window.OnShow();
            return window;
        }

        public InventoryInventoryWindow ShowInventoryInventoryWindow(EquipmentModel equipmentModel, InventoryModel firstInventoryModel, InventoryModel secondInentoryModel, ItemCommandHandler handler)
        {
            var window = InstantiateWindow<InventoryInventoryWindow>();
            var controller = new InventoryInventoryWindowController(this, window, equipmentModel, firstInventoryModel, secondInentoryModel, handler);
            PushToStack(window, controller);
            window.OnShow();
            return window;
        }

        public ItemStacksPreviewWindow ShowItemStackPreviewWindow<T1, T2>(EquipmentModel equipmentModel, EquipmentSlotKey? equipmentSlot, T1 primarySlot, IItemContainer<T1> primaryItemContainer, IItemContainer<T2> secondaryItemContainer, ItemCommandHandler handler)
        {
            var window = InstantiateWindow<ItemStacksPreviewWindow>();
            var controller = new ItemStacksPreviewWindowController<T1, T2>(this, window, equipmentModel, equipmentSlot, primarySlot, primaryItemContainer, secondaryItemContainer, handler);
            PushToStack(window, controller);
            window.OnShow();
            return window;
        }

        private void PushToStack(UIWindow window, IDisposable controller)
        {
            _windowStack.Push((window, controller));
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
