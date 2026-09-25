using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.DataFormatters;
using Assets._Game.Scripts.UI.Systems;
using Assets._Game.Scripts.UI.Systems.DragDrop;
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
        private EntityNameplateWidget _entityNameplateView;
        [SerializeField]
        private UIWindowBase[] _windowPrefabs;
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
        private InteractionPromptWidget _interactionPromptView;
        [SerializeField]
        private ClickEffectWidget _clickEffectView;
        [SerializeField]
        private DragDropWidget _dragDropView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_entityNameplateView);
            builder.RegisterInstance(_rootReferences);

            builder.RegisterEntryPoint<UIBootstrap>(Lifetime.Scoped);
            builder.RegisterEntryPoint<UISystemRunner>(Lifetime.Scoped);

            builder.Register<EquipmentViewData>(Lifetime.Transient);
            builder.Register<InventoryViewData>(Lifetime.Transient);
            builder.Register<StorageHudData>(Lifetime.Transient);
            builder.Register<CraftingWindowData>(Lifetime.Transient);
            builder.Register<QuestsWindowData>(Lifetime.Transient);
            builder.Register<QuestGiverWindowData>(Lifetime.Transient);
            builder.Register<StatsViewData>(Lifetime.Transient);

            builder.Register<CheatsWindowData>(Lifetime.Transient);

            RegisterSystems(builder);
            RegisterWindows(builder);
            RegisterHud(builder);
            RegisterItemContainers(builder);
            RegisterDataFormatters(builder);
            RegisterServices(builder);
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<DragDropHandler>(Lifetime.Singleton);
        }

        private void RegisterSystems(IContainerBuilder builder)
        {
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

        private void RegisterWindows(IContainerBuilder builder)
        {
            builder.Register<WindowManager>(Lifetime.Scoped);
            builder.RegisterInstance(_windowWrapperPrefab);
            builder.RegisterInstance(_modalWrapperPrefab);
            builder.RegisterInstance((IEnumerable<UIWindowBase>)_windowPrefabs);

            builder.Register<WindowControllerArgumentsProvider>(Lifetime.Singleton);

            // Register Window Definitions and Window Controllers
            var windows = new WindowDefinition[]
            {
                new(WindowId.Inventory, typeof(InventoryWindowController), new(true, false, true)),

                // Primary windows
                new(WindowId.Cheats, typeof(CheatsWindowController), new(true, false, true)),
                new(WindowId.Equipment, typeof(EquipmentWindowController), new(true, false, true)),
                new(WindowId.Quests, typeof(QuestsWindowController), new(true, false, true)),
                new(WindowId.QuestGiver, typeof(QuestGiverWindowController), new(true, false, true)),
                new(WindowId.QuestDescription, typeof(QuestDescriptionWindowController), new(true, false, true)),

                new(WindowId.Stats, typeof(StatsWindowController), new(true, false, true)),
                new(WindowId.Storage, typeof(StorageWindowController), new(true, false, true)),
                new(WindowId.LocationTransitionList, typeof(LocationTransitionListWindowController), new(true, false, true)),
                new(WindowId.Crafting, typeof(CraftingWindowController), new(true, false, true)),
                new(WindowId.Shop, typeof(ShopWindowController), new(true, false, true)),

                // Service windows
                new(WindowId.ItemUseSettings, typeof(ItemUseSettingsWindowController), new(true, false, true)),
                new(WindowId.ItemStacksPreview, typeof(ItemStacksPreviewWindowController), new(true, false, true)),

                new(WindowId.AmountPicker, typeof(AmountPickerWindowController), new(true, true, false)),
                new(WindowId.Confirmation, typeof(ConfirmationWindowController), new(true, true, false)),
            };

            foreach (var windowDefinition in windows)
            {
                builder.Register(windowDefinition.ControllerType, Lifetime.Transient);
            }

            builder.RegisterInstance((IEnumerable<WindowDefinition>)windows);
        }

        private void RegisterHud(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<CompactPlayerStateView>();
            builder.Register<CompactPlayerStateViewController>(Lifetime.Scoped);
            builder.Register<PlayerStateViewData>(Lifetime.Transient);

            builder.RegisterComponentInHierarchy<PlayerAiToggleView>();
            builder.Register<PlayerAiToggleViewController>(Lifetime.Scoped);

            builder.RegisterComponent(_locationAnnounceView);
            builder.RegisterComponent(_interactionPromptView);
            builder.RegisterComponent(_clickEffectView);
            builder.RegisterComponent(_dragDropView);
        }

        private void RegisterItemContainers(IContainerBuilder builder)
        {
            builder.Register<InventoryViewController>(Lifetime.Transient);
            builder.Register<EquipmentViewController>(Lifetime.Transient);
            builder.Register<ShopViewController>(Lifetime.Transient);
            builder.Register<LocationTransitionListViewController>(Lifetime.Transient);
            builder.Register<StatsViewController>(Lifetime.Transient);
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
    }
}
