using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.DataFormatters;
using Assets._Game.Scripts.UI.Services;
using Assets._Game.Scripts.UI.Systems;
using Assets._Game.Scripts.UI.Systems.DragDrop;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;
using Assets._Game.Scripts.UI.Windows.Controllers.ItemPreview;
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
        private EntityNameplateView _entityNameplateView;
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
        private LocationAnnounceView _locationAnnounceView;
        [SerializeField]
        private InteractionPromptView _interactionPromptView;
        [SerializeField]
        private ClickEffectView _clickEffectView;
        [SerializeField]
        private DragDropView _dragDropView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_entityNameplateView);
            builder.RegisterInstance(_rootReferences);

            builder.RegisterEntryPoint<UIBootstrap>(Lifetime.Scoped);

            builder.Register<EquipmentHudData>(Lifetime.Transient);
            builder.Register<InventoryHudData>(Lifetime.Transient);
            builder.Register<StorageHudData>(Lifetime.Transient);
            builder.Register<CraftingHudData>(Lifetime.Transient);
            builder.Register<QuestsHudData>(Lifetime.Transient);
            builder.Register<QuestGiverHudData>(Lifetime.Transient);

            builder.Register<CheatsHudData>(Lifetime.Transient);

            RegisterSystems(builder);
            RegisterWindows(builder);
            RegisterHud(builder);
            RegisterItemContainers(builder);
            RegisterDataFormatters(builder);
            RegisterServices(builder);
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<ItemPreviewService>(Lifetime.Singleton);

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

            var windows = new WindowDefinition[]
            {
                // Primary windows
                new(WindowId.Cheats, new WindowConfiguration(true, false, true), typeof(CheatsWindow), typeof(CheatsWindowController), typeof(CheatsWindowOpenStrategy)),
                new(WindowId.Equipment, new WindowConfiguration(true, false, true), typeof(InventoryEquipmentWindow), typeof(InventoryEquipmentWindowController), typeof(InventoryEquipmentWindowOpenStrategy)),
                new(WindowId.Quests, new WindowConfiguration(true, false, true), typeof(QuestsWindow), typeof(QuestsWindowController), typeof(QuestsWindowOpenStrategy)),
                new(WindowId.QuestGiver, new WindowConfiguration(true, false, true), typeof(QuestGiverWindow), typeof(QuestGiverWindowController)),
                new(WindowId.QuestDescription, new WindowConfiguration(true, false, true), typeof(QuestDescriptionWindow), typeof(QuestDescriptionWindowController)),

                new(WindowId.Stats, new WindowConfiguration(true, false, true), typeof(StatsWindow), typeof(StatsWindowController)),
                new(WindowId.Storage, new WindowConfiguration(true, false, true), typeof(InventoryStorageWindow), typeof(InventoryStorageWindowController)),
                new(WindowId.LocationTransitionList, new WindowConfiguration(true, false, true), typeof(LocationTransitionListWindow), typeof(LocationTransitionListWindowController)),
                new(WindowId.Crafting, new WindowConfiguration(true, false, true), typeof(CraftingWindow), typeof(CraftingWindowController)),
                new(WindowId.Shop, new WindowConfiguration(true, false, true), typeof(InventoryShopWindow), typeof(InventoryShopWindowController)),

                // Service windows
                new(WindowId.ItemUseSettings, new WindowConfiguration(true, false, true), typeof(ItemUseSettingsWindow), typeof(ItemUseSettingsWindowController), typeof(ItemUseSettingsWindowOpenStrategy)),
                new(WindowId.ItemStacksPreview, new WindowConfiguration(true, false, true), typeof(ItemStacksPreviewWindow), typeof(ItemStacksPreviewWindowController)),

                new(WindowId.AmountPicker, new WindowConfiguration(true, true, false), typeof(AmountPickerWindow), typeof(AmountPickerWindowController)),
                new(WindowId.Confirmation, new WindowConfiguration(true, true, false), typeof(ConfirmationWindow), typeof(ConfirmationWindowController)),
            };

            foreach (var windowDefinition in windows)
            {
                builder.Register(windowDefinition.ControllerType, Lifetime.Transient);
                if (windowDefinition.StrategyType != null)
                    builder.Register(windowDefinition.StrategyType, Lifetime.Singleton);
            }

            builder.RegisterInstance((IEnumerable<UIWindowBase>)_windowPrefabs);
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
