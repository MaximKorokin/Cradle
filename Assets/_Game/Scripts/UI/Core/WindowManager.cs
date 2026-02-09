using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.Core;
using Assets._Game.Scripts.UI.Views;
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

        private T ShowWindow<T, K>(Func<T, K> controllerFactory) where T : UIWindow where K : IDisposable
        {
            var window = InstantiateWindow<T>();
            var controller = controllerFactory(window);
            _windowStack.Push((window, controller));
            window.OnShow();
            return window;
        }

        public InventoryEquipmentWindow ShowInventoryEquipmentWindow(InventoryModel inventoryModel, EquipmentModel equipmentModel, ItemCommandHandler handler)
        {
            return ShowWindow<InventoryEquipmentWindow, InventoryEquipmentWindowController>(
                w => new InventoryEquipmentWindowController(this, w, inventoryModel, equipmentModel, handler));
        }

        public InventoryInventoryWindow ShowInventoryInventoryWindow(EquipmentModel equipmentModel, InventoryModel firstInventoryModel, InventoryModel secondInentoryModel, ItemCommandHandler handler)
        {
            return ShowWindow<InventoryInventoryWindow, InventoryInventoryWindowController>(
                w => new InventoryInventoryWindowController(this, w, equipmentModel, firstInventoryModel, secondInentoryModel, handler));
        }

        public ItemStacksPreviewWindow ShowItemStackPreviewWindow<T1, T2>(EquipmentModel equipmentModel, EquipmentSlotKey? equipmentSlot, T1 primarySlot, IItemContainer<T1> primaryItemContainer, IItemContainer<T2> secondaryItemContainer, ItemCommandHandler handler)
        {
            return ShowWindow<ItemStacksPreviewWindow, ItemStacksPreviewWindowController<T1, T2>>(
                w => new ItemStacksPreviewWindowController<T1, T2>(this, w, equipmentModel, equipmentSlot, primarySlot, primaryItemContainer, secondaryItemContainer, handler));
        }

        public StatsWindow ShowStatsWindow(StatsModule statsModule)
        {
            return ShowWindow<StatsWindow, StatsWindowController>(w => new StatsWindowController(w, statsModule));
        }

        public CheatsWindow ShowCheatsWindow(ItemDefinitionCatalog itemDefinitionCatalog, ItemStackAssembler itemStackAssembler, PlayerContext playerContext)
        {
            return ShowWindow<CheatsWindow, CheatsWindowController>(w => new CheatsWindowController(w, itemDefinitionCatalog, itemStackAssembler, playerContext));
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
