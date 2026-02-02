using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Windows
{
    public class WindowManager
    {
        private readonly RectTransform _windowsRoot;
        private readonly InventoryEquipmentWindow _inventoryEquipmentWindowPrefab;
        private readonly InventoryInventoryWindow _inventoryInventoryWindowPrefab;
        private readonly Stack<(UIWindow Window, IDisposable Controller)> _stack = new();
        private readonly IObjectResolver _resolver;

        public WindowManager(
            InventoryEquipmentWindow inventoryEquipmentWindowPrefab,
            InventoryInventoryWindow inventoryInventoryWindowPrefab,
            UIRootReferences rootReferences,
            IObjectResolver resolver)
        {
            _inventoryEquipmentWindowPrefab = inventoryEquipmentWindowPrefab;
            _inventoryInventoryWindowPrefab = inventoryInventoryWindowPrefab;
            _windowsRoot = rootReferences.WindowsRoot;
            _resolver = resolver;
        }

        public InventoryEquipmentWindow ShowInventoryEquipmentWindow(InventoryModel inventoryModel, EquipmentModel equipmentModel, ItemCommandHandler handler)
        {
            var window = _resolver.Instantiate(_inventoryEquipmentWindowPrefab, _windowsRoot);
            var controller = new InventoryEquipmentWindowController(window, inventoryModel, equipmentModel, handler);
            _stack.Push((window, controller));
            window.OnShow();
            return window;
        }

        public InventoryInventoryWindow ShowInventoryInventoryWindow(InventoryModel firstInventoryModel, InventoryModel secondInentoryModel, ItemCommandHandler handler)
        {
            var window = _resolver.Instantiate(_inventoryInventoryWindowPrefab, _windowsRoot);
            var controller = new InventoryInventoryWindowController(window, firstInventoryModel, secondInentoryModel, handler);
            _stack.Push((window, controller));
            window.OnShow();
            return window;
        }

        public void CloseTop()
        {
            if (_stack.Count == 0) return;
            var (window, controller) = _stack.Pop();
            window.OnHide();
            UnityEngine.Object.Destroy(window.gameObject);
            controller.Dispose();
        }
    }
}
