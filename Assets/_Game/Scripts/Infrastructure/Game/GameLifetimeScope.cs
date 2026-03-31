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

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameBootstrap>();

            builder.RegisterComponentInHierarchy<AppLifecycleBridge>();
            builder.Register<IAppLifecycleHandler, AppLifecycleHandler>(Lifetime.Transient);
            builder.Register<IGlobalEventBus, GlobalEventBus>(Lifetime.Singleton);

            builder.Register<PoolService>(Lifetime.Scoped);
            builder.RegisterInstance(_defaultPrefabReferences);
            builder.RegisterInstance(_defaultEntityDefinitionReferences);

            builder.Register<DispatcherService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<UnityWorldQuery>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<EntitySensor>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<PlayerControlProvider>(Lifetime.Singleton);
            builder.Register<PlayerContext>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<LocationManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<LocationCatalog>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

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
        }

        private void RegisterSystems(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<SystemsRunner>();

            builder.Register<PlayerSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<LocomotionSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AppearanceSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<StatSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ActionSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ControlSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<StatusEffectSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<RewardSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<LootSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<DespawnSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<LevelingSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AttackModifierSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EquipmentSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ItemSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<InteractionSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EntityPlacementSystem>(Lifetime.Singleton).AsImplementedInterfaces();
        }

        private void RegisterSavesFeature(IContainerBuilder builder)
        {
            builder.Register<SaveService>(Lifetime.Scoped);
            builder.Register<GameSaveRepository>(Lifetime.Scoped);
            builder.Register<ISaveStorage, PlayerPrefsSavesStorage>(Lifetime.Scoped);
            builder.Register<ISaveSerializer, JsonSaveSerializer>(Lifetime.Scoped);

            builder.Register<IDataCodec, CooldownCodec>(Lifetime.Scoped);
            builder.Register<IDataCodec, DurabilityCodec>(Lifetime.Scoped);
            builder.Register<IDataCodec, EmptyCodec>(Lifetime.Scoped);
            builder.Register<CodecRegistry>(Lifetime.Scoped);
        }

        private void RegisterEntityFeature(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EntityViewLifecycleOrchestrator>();

            builder.Register<StatusEffectDefinitionCatalog>(Lifetime.Singleton);

            builder.Register<EntityDefinitionCatalog>(Lifetime.Scoped);
            builder.Register<EntityRepository>(Lifetime.Scoped);
            builder.Register<EntityFactory>(Lifetime.Scoped);

            builder.Register<EntityViewProvider>(Lifetime.Scoped);
            builder.Register<EntityVisualModelCatalog>(Lifetime.Scoped);
            builder.Register<UnitVariantsCatalog>(Lifetime.Scoped);

            builder.Register<FactionRelationResolver>(Lifetime.Singleton);
            builder.RegisterInstance(_factionRelations);

            builder.Register<ActionEvaluator>(Lifetime.Singleton);
        }

        private void RegisterEntityModuleFactories(IContainerBuilder builder)
        {
            builder.Register<StatsControllerAssembler>(Lifetime.Scoped);
            builder.Register<StatModuleFactory>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();

            builder.Register<AppearanceModuleFactory>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();

            builder.Register<StorageModuleFactory>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            builder.Register<InventoryModuleFactory>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            builder.Register<EquipmentModuleFactory>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();

            builder.Register<UnitViewProvider>(Lifetime.Scoped);
            builder.Register<UnitsControllerFactory>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();

            builder.Register<ControlModuleFactory>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            builder.Register<AiBrainFactory>(Lifetime.Scoped);
            builder.Register<IntentModuleFactory>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            builder.Register<KinematicsModuleFactory>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            builder.Register<SpatialModuleFactory>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();

            builder.Register<RestrictionStateModuleFactory>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();

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
            builder.Register<ItemInstanceDataFactory>(Lifetime.Scoped);
            builder.Register<ItemStackFactory>(Lifetime.Scoped);
            builder.Register<ItemDefinitionCatalog>(Lifetime.Scoped);
            builder.Register<ItemCommandHandler>(Lifetime.Scoped);

            builder.Register<InventoryModelFactory>(Lifetime.Scoped);
            builder.Register<EquipmentModelFactory>(Lifetime.Scoped);
        }
    }
}
