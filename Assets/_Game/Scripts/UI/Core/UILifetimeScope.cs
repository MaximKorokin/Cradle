using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.DataFormatters;
using Assets._Game.Scripts.UI.Systems;
using Assets._Game.Scripts.UI.Systems.Click;
using Assets._Game.Scripts.UI.Systems.DragDrop;
using Assets._Game.Scripts.UI.Systems.Tooltip;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;
using Assets._Game.Scripts.UI.Views.Widgets;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;
using Assets._Game.Scripts.UI.Windows.Modal;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public class UILifetimeScope : LifetimeScope
    {
        [Header("Systems")]
        [SerializeField]
        private Transform _uiSystemsRoot;
        [Header("Prefabs")]
        [SerializeField]
        private EntityNameplateWidget _entityNameplateViewPrefab;
        [SerializeField]
        private WindowWrapper _windowWrapperPrefab;
        [SerializeField]
        private ModalWrapper _modalWrapperPrefab;
        [Space]
        [Header("MonoBehaviours")]
        [SerializeField]
        private UIRootReferences _rootReferences;
        [SerializeField]
        private LocationAnnounceWidget _locationAnnounceView;
        [SerializeField]
        private PlayerPromptWidget _interactionPromptView;
        [SerializeField]
        private ClickEffectWidget _clickEffectView;
        [SerializeField]
        private DragDropWidget _dragDropView;
        [SerializeField]
        private TooltipWidget _tooltipView;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterInstances(builder);
            RegisterSystems(builder);
            RegisterDataAggregators(builder);
            RegisterViewControllers(builder);
            RegisterDataFormatters(builder);
            RegisterWindows(builder);
            RegisterHud(builder);
            RegisterServices(builder);
        }

        private void RegisterInstances(IContainerBuilder builder)
        {
            builder.RegisterInstance(_entityNameplateViewPrefab);
            builder.RegisterInstance(_rootReferences);

            builder.RegisterInstance(_windowWrapperPrefab);
            builder.RegisterInstance(_modalWrapperPrefab);
        }

        private void RegisterSystems(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<UIBootstrap>(Lifetime.Scoped);
            builder.RegisterEntryPoint<UISystemRunner>(Lifetime.Scoped);

            var uiSystems = _uiSystemsRoot.GetComponentsInChildren<UISystemBase>(true).ToArray();

            foreach (var system in uiSystems)
            {
                builder.RegisterComponent(system).AsSelf().AsImplementedInterfaces();
            }

            builder.RegisterBuildCallback(container =>
            {
                foreach (var system in uiSystems)
                {
                    container.Resolve(system.GetType());
                }
            });
        }

        private void RegisterDataAggregators(IContainerBuilder builder)
        {
            builder.Register<EquipmentViewData>(Lifetime.Transient);
            builder.Register<InventoryViewData>(Lifetime.Transient);
            builder.Register<StorageViewData>(Lifetime.Transient);
            builder.Register<StatsViewData>(Lifetime.Transient);
            builder.Register<EntityAiToggleViewData>(Lifetime.Transient);
            builder.Register<QuestsViewData>(Lifetime.Transient);
            builder.Register<QuestGiverViewData>(Lifetime.Transient);
            builder.Register<CraftingViewData>(Lifetime.Transient);
            builder.Register<CheatsViewData>(Lifetime.Transient);
            builder.Register<LocationTransitionListViewData>(Lifetime.Transient);
            builder.Register<QuestDescriptionViewData>(Lifetime.Transient);
            builder.Register<ShopViewData>(Lifetime.Transient);
            builder.Register<ItemUseSettingsViewData>(Lifetime.Transient);
            builder.Register<ItemStackPreviewViewData>(Lifetime.Transient);
            builder.Register<StatusEffectListViewData>(Lifetime.Transient);
            builder.Register<EntityStatusViewData>(Lifetime.Transient);
        }

        private void RegisterViewControllers(IContainerBuilder builder)
        {
            builder.Register<InventoryViewController>(Lifetime.Transient);
            builder.Register<EquipmentViewController>(Lifetime.Transient);
            builder.Register<ShopViewController>(Lifetime.Transient);
            builder.Register<StatsViewController>(Lifetime.Transient);
            builder.Register<QuestsViewController>(Lifetime.Transient);
            builder.Register<CheatsViewController>(Lifetime.Transient);
            builder.Register<CraftingViewController>(Lifetime.Transient);
            builder.Register<QuestGiverViewController>(Lifetime.Transient);
            builder.Register<LocationTransitionListViewController>(Lifetime.Transient);
            builder.Register<QuestDescriptionViewController>(Lifetime.Transient);
            builder.Register<CompactEntityStatusViewController>(Lifetime.Transient);
            builder.Register<EntityAiToggleViewController>(Lifetime.Transient);
            builder.Register<ItemUseSettingsViewController>(Lifetime.Transient);
            builder.Register<ItemStackPreviewViewController>(Lifetime.Transient);
            builder.Register<StatusEffectListViewController>(Lifetime.Transient);
            builder.Register<CompactEntityStatusViewController>(Lifetime.Transient);
        }

        private void RegisterDataFormatters(IContainerBuilder builder)
        {
            builder.Register<ItemStackFormatter>(Lifetime.Singleton);
            builder.Register<ItemDefinitionFormatter>(Lifetime.Singleton);
            builder.Register<ItemSetFormatter>(Lifetime.Singleton);
            builder.Register<FunctionalItemTraitFormatter>(Lifetime.Singleton);
            builder.Register<EnchantableTraitFormatter>(Lifetime.Singleton);
            builder.Register<StatModifiersFormatter>(Lifetime.Singleton);
            builder.Register<AttackModifiersFormatter>(Lifetime.Singleton);
            builder.Register<StatusEffectFormatter>(Lifetime.Singleton);
            builder.Register<InteractionDefinitionFormatter>(Lifetime.Singleton);
            builder.Register<ActionDefinitionFormatter>(Lifetime.Singleton);
            builder.Register<QuestStateFormatter>(Lifetime.Singleton);
            builder.Register<QuestObjectiveProgressFormatter>(Lifetime.Singleton);
        }

        private void RegisterWindows(IContainerBuilder builder)
        {
            builder.Register<WindowManager>(Lifetime.Scoped);

            builder.Register<WindowControllerArgumentsProvider>(Lifetime.Singleton);

            var windows = WindowDefinitionScanner.Scan();

            foreach (var windowDefinition in windows)
            {
                builder.Register(windowDefinition.ControllerType, Lifetime.Transient);
            }

            builder.RegisterInstance((IEnumerable<WindowDefinition>)windows);
            builder.Register<WindowRegistry>(Lifetime.Singleton);
        }

        private void RegisterHud(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<InitialWindowsDataProvider>();

            builder.RegisterComponent(_locationAnnounceView);
            builder.RegisterComponent(_interactionPromptView);
            builder.RegisterComponent(_clickEffectView);
            builder.RegisterComponent(_dragDropView);
            builder.RegisterComponent(_tooltipView);
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<DragDropHandlerService>(Lifetime.Singleton);
            builder.Register<ClickHandlerService>(Lifetime.Singleton);
            builder.Register<TooltipHandlerService>(Lifetime.Singleton);
            builder.Register<WindowsInitializerService>(Lifetime.Singleton);
        }
    }
}
