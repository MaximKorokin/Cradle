using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Systems;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;
using Assets._Game.Scripts.UI.Windows.Modal;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public class UILifetimeScope : LifetimeScope
    {
        [Header("Prefabs")]
        [SerializeField]
        private EntityNameplateView _entityNameplateView;
        [SerializeField]
        private UIWindowBase[] _windowPrefabs;
        [SerializeField]
        private ModalWrapper _modalWrapperPrefab;
        [Space]
        [Header("MonoBehaviours")]
        [SerializeField]
        private UIRootReferences _rootReferences;
        [SerializeField]
        private LocationAnnounceView _locationAnnounceView;
        [SerializeField]
        private LocationTransitionView _locationTransitionView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_entityNameplateView);
            builder.RegisterInstance(_rootReferences);

            builder.RegisterComponent(_locationAnnounceView);
            builder.RegisterComponent(_locationTransitionView);

            builder.RegisterEntryPoint<UIBootstrap>(Lifetime.Scoped);

            builder.Register<EquipmentHudData>(Lifetime.Transient);
            builder.Register<InventoryHudData>(Lifetime.Transient);
            builder.Register<StorageHudData>(Lifetime.Transient);
            builder.Register<CheatsHudData>(Lifetime.Transient);

            RegisterSystems(builder);
            RegisterWindows(builder);
            RegisterHud(builder);
            RegisterItemContainers(builder);
        }

        private void RegisterSystems(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<EntityNameplateUISystem>();
            builder.RegisterComponentInHierarchy<LocationAnnounceUISystem>();
            builder.RegisterComponentInHierarchy<LocationTransitionUISystem>();
            builder.RegisterComponentInHierarchy<FloatingTextUISystem>();
            builder.RegisterComponentInHierarchy<PlayerReviveUISystem>();
        }

        private void RegisterWindows(IContainerBuilder builder)
        {
            builder.Register<WindowManager>(Lifetime.Scoped);
            builder.RegisterInstance(_modalWrapperPrefab);

            var windows = new WindowDefinition[]
            {
                // Primary windows
                new(WindowId.Cheats, typeof(CheatsWindow), typeof(CheatsWindowController)),
                new(WindowId.Stats, typeof(StatsWindow), typeof(StatsWindowController)),
                new(WindowId.InventoryInventory, typeof(InventoryInventoryWindow), typeof(InventoryInventoryWindowController)),
                new(WindowId.InventoryEquipment, typeof(InventoryEquipmentWindow), typeof(InventoryEquipmentWindowController)),
                new(WindowId.LocationTransitionList, typeof(LocationTransitionListWindow), typeof(LocationTransitionListWindowController)),

                // Service windows
                new(WindowId.ItemStacksPreview, typeof(ItemStacksPreviewWindow), typeof(ItemStacksPreviewWindowController)),
                new(WindowId.ItemUseSettings, typeof(ItemUseSettingsWindow), typeof(ItemUseSettingsWindowController)),
                new(WindowId.AmountPicker, typeof(AmountPickerWindow), typeof(AmountPickerWindowController)),
            };

            foreach (var windowDefinition in windows)
            {
                builder.Register(windowDefinition.ControllerType, Lifetime.Transient);
            }

            builder.RegisterInstance((IEnumerable<UIWindowBase>)_windowPrefabs);
            builder.RegisterInstance((IEnumerable<WindowDefinition>)windows);
        }

        private void RegisterHud(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<CompactPlayerStateView>();
            builder.Register<CompactPlayerStateViewController>(Lifetime.Scoped);
            builder.Register<PlayerStateViewData>(Lifetime.Transient);

            builder.RegisterComponentInHierarchy<PlayerReviveView>();
        }

        private void RegisterItemContainers(IContainerBuilder builder)
        {
            builder.Register<InventoryViewController>(Lifetime.Transient);
            builder.Register<EquipmentViewController>(Lifetime.Transient);
        }
    }
}
