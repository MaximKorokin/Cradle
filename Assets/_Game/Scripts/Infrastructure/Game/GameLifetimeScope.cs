using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Entities.Control.AI;
using Assets._Game.Scripts.Entities.Faction;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Calculators;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Infrastructure.Persistence.Codecs;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Infrastructure.Systems.Location;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Locations;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private ConfigReferences _configReferences;
        [SerializeField]
        private FactionRelations _factionRelations;
        [SerializeField]
        private DefaultPrefabReferences _defaultPrefabReferences;
        [SerializeField]
        private DefaultEntityDefinitionReferences _defaultEntityDefinitionReferences;
        [Space]
        [SerializeField]
        private Camera _mainCamera;
        [SerializeField]
        private CinemachineCameraService _cinemachineCameraService;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameBootstrap>();

            builder.RegisterComponentInHierarchy<AppLifecycleBridge>();
            builder.Register<IAppLifecycleHandler, AppLifecycleHandler>(Lifetime.Transient);
            builder.Register<IGlobalEventBus, GlobalEventBus>(Lifetime.Singleton);

            builder.Register<PoolService>(Lifetime.Singleton);
            builder.RegisterInstance(_defaultPrefabReferences);
            builder.RegisterInstance(_defaultEntityDefinitionReferences);

            builder.RegisterComponent(_mainCamera);
            builder.RegisterComponent(_cinemachineCameraService).AsImplementedInterfaces().AsSelf();

            builder.Register<DispatcherService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<UnityWorldQuery>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<EntitySensor>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<PlayerControlProvider>(Lifetime.Singleton);
            builder.Register<PlayerContext>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<LocationManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<LocationCatalog>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<LocationEntranceCatalog>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            RegisterConfigs(builder);
            RegisterSystems(builder);
            RegisterSavesFeature(builder);
            RegisterEntityFeature(builder);
            RegisterEntityModuleFactories(builder);
            RegisterItemFeature(builder);

            RegisterCalculators(builder);
        }

        private void RegisterConfigs(IContainerBuilder builder)
        {
            builder.RegisterInstance(_configReferences.NewGameDefinition);

            builder.RegisterInstance(_configReferences.StatusEffectsConfig);
            builder.RegisterInstance(_configReferences.ItemsConfig);
            builder.RegisterInstance(_configReferences.EntityUnitConfig);
            builder.RegisterInstance(_configReferences.StatsConfig);
            builder.RegisterInstance(_configReferences.SaveConfig);
            builder.RegisterInstance(_configReferences.DespawnConfig);
            builder.RegisterInstance(_configReferences.LevelingConfig);
            builder.RegisterInstance(_configReferences.LootConfig);
            builder.RegisterInstance(_configReferences.LocationConfig);
            builder.RegisterInstance(_configReferences.FloatingTextConfig);
            builder.RegisterInstance(_configReferences.ReviveConfig);
        }

        private void RegisterSystems(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<SystemRunner>();

            builder.Register<LocomotionSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AppearanceSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<StatSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ActionSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ControlSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<StatusEffectSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<RewardSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<LootSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EntityLifecycleSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<LevelingSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AttackModifierSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EquipmentSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ItemSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<InteractionSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EntityPlacementSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<LocationTransitionSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<CameraFollowSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<FloatingTextSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EntityReviveSystem>(Lifetime.Singleton).AsImplementedInterfaces();
        }

        private void RegisterSavesFeature(IContainerBuilder builder)
        {
            builder.Register<SaveService>(Lifetime.Singleton);
            builder.Register<GameSaveRepository>(Lifetime.Singleton);
            builder.Register<ISaveStorage, PlayerPrefsSavesStorage>(Lifetime.Singleton);
            builder.Register<ISaveSerializer, JsonSaveSerializer>(Lifetime.Singleton);

            builder.Register<IDataCodec, CooldownCodec>(Lifetime.Singleton);
            builder.Register<IDataCodec, DurabilityCodec>(Lifetime.Singleton);
            builder.Register<IDataCodec, EmptyCodec>(Lifetime.Singleton);
            builder.Register<CodecRegistry>(Lifetime.Singleton);
        }

        private void RegisterEntityFeature(IContainerBuilder builder)
        {
            builder.Register<EntityViewService>(Lifetime.Singleton);

            builder.Register<StatusEffectDefinitionCatalog>(Lifetime.Singleton);

            builder.Register<EntityDefinitionCatalog>(Lifetime.Singleton);
            builder.Register<EntityRepository>(Lifetime.Singleton);
            builder.Register<EntityFactory>(Lifetime.Singleton);

            builder.Register<EntityViewProvider>(Lifetime.Singleton);
            builder.Register<EntityVisualModelCatalog>(Lifetime.Singleton);
            builder.Register<UnitVariantsCatalog>(Lifetime.Singleton);

            builder.Register<FactionRelationResolver>(Lifetime.Singleton);
            builder.RegisterInstance(_factionRelations);

            builder.Register<ActionEvaluator>(Lifetime.Singleton);
        }

        private void RegisterEntityModuleFactories(IContainerBuilder builder)
        {
            builder.Register<StatsControllerAssembler>(Lifetime.Singleton);
            builder.Register<StatModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<HealthModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<AppearanceModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<StorageModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<InventoryModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<EquipmentModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<UnitViewProvider>(Lifetime.Singleton);
            builder.Register<UnitsControllerFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<ControlModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<AiBrainFactory>(Lifetime.Singleton);
            builder.Register<IntentModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<KinematicsModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<SpatialModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<RestrictionStateModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<FactionModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<ActionModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<RewardModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<DespawnModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<WanderModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<LevelingModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<StatusEffectModuleFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }

        private void RegisterCalculators(IContainerBuilder builder)
        {
            builder.Register<IDamageCalculator, DamageCalculator>(Lifetime.Singleton);
            builder.Register<DerivedStatsCalculator>(Lifetime.Singleton);
        }

        private void RegisterItemFeature(IContainerBuilder builder)
        {
            builder.Register<ItemInstanceDataFactory>(Lifetime.Singleton);
            builder.Register<ItemStackFactory>(Lifetime.Singleton);
            builder.Register<ItemDefinitionCatalog>(Lifetime.Singleton);
            builder.Register<ItemCommandHandler>(Lifetime.Singleton);

            builder.Register<InventoryModelFactory>(Lifetime.Singleton);
            builder.Register<EquipmentModelFactory>(Lifetime.Singleton);

            builder.Register<ItemContainerResolver>(Lifetime.Singleton);
        }
    }
}
