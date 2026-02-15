using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
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
        [Header("Windows")]
        [SerializeField]
        private UIWindowBase[] _windowPrefabs;
        [SerializeField]
        private ModalWrapper _modalWrapperPrefab;
        [Space]
        [Header("MonoBehaviours")]
        [SerializeField]
        private UIRootReferences _rootReferences;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<HudView>();
            builder.Register<HudViewController>(Lifetime.Scoped);
            builder.RegisterInstance(_rootReferences);

            builder.RegisterEntryPoint<UIBootstrap>(Lifetime.Scoped);

            builder.Register<EquipmentHudData>(Lifetime.Transient);
            builder.Register<InventoryHudData>(Lifetime.Transient);
            builder.Register<StashHudData>(Lifetime.Transient);
            builder.Register<CheatsHudData>(Lifetime.Transient);

            RegisterWindows(builder);
        }

        private void RegisterWindows(IContainerBuilder builder)
        {
            builder.Register<WindowManager>(Lifetime.Scoped);
            builder.RegisterInstance(_modalWrapperPrefab);
            builder.RegisterComponentInHierarchy<WindowOpenTrigger>();

            var windows = new WindowDefinition[]
            {
                new(WindowId.Cheats, typeof(CheatsWindow), typeof(CheatsWindowController)),
                new(WindowId.Stats, typeof(StatsWindow), typeof(StatsWindowController)),
                new(WindowId.InventoryInventory, typeof(InventoryInventoryWindow), typeof(InventoryInventoryWindowController)),
                new(WindowId.InvestoryEquipment, typeof(InventoryEquipmentWindow), typeof(InventoryEquipmentWindowController)),
                new(WindowId.Pause, typeof(InventoryEquipmentWindow), typeof(InventoryEquipmentWindowController)),

                new(WindowId.None, typeof(ItemStacksPreviewWindow), typeof(ItemStacksPreviewWindowController<, >)),
            };

            foreach (var windowDefinition in windows)
            {
                builder.Register(windowDefinition.GetControllerType(), Lifetime.Transient);
            }

            builder.RegisterInstance((IEnumerable<UIWindowBase>)_windowPrefabs);
            builder.RegisterInstance((IEnumerable<WindowDefinition>)windows);
        }
    }
}
