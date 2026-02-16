using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Infrastructure.Persistence.Codecs;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private NewGameDefinition _newGameDefinition;
        [SerializeField]
        private ItemsConfig _itemsConfig;
        [SerializeField]
        private StatsConfig _statsConfig;
        [SerializeField]
        private StatusEffectsConfig _statusEffectsConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameBootstrap>();
            builder.Register<Dispatcher>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.RegisterInstance(_newGameDefinition);

            builder.Register<PlayerContext>(Lifetime.Singleton);

            RegisterSavesFeature(builder);
            RegisterEntityFeature(builder);
            RegisterItemFeature(builder);
            RegisterStatusEffectFeature(builder);
        }

        private void RegisterSavesFeature(IContainerBuilder builder)
        {
            builder.Register<SaveService>(Lifetime.Scoped);
            builder.Register<GameSaveRepository>(Lifetime.Scoped);
            builder.Register<ISaveStorage, PlayerPrefsSavesStorage>(Lifetime.Scoped);
            builder.Register<ISaveSerializer, JsonSaveSerializer>(Lifetime.Scoped);

            builder.Register<IDataCodec, DurabilityCodec>(Lifetime.Scoped);
            builder.Register<IDataCodec, EmptyCodec>(Lifetime.Scoped);
            builder.Register<CodecRegistry>(Lifetime.Scoped);
        }

        private void RegisterEntityFeature(IContainerBuilder builder)
        {
            builder.Register<EntityDefinitionCatalog>(Lifetime.Scoped);
            builder.Register<EntityRepository>(Lifetime.Scoped);
            builder.Register<EntityAssembler>(Lifetime.Scoped);

            builder.RegisterInstance(_statsConfig);
            builder.Register<StatsModuleAssembler>(Lifetime.Scoped);
            builder.Register<StatsControllerAssembler>(Lifetime.Scoped);

            builder.Register<AppearanceModuleFactory>(Lifetime.Scoped);
            builder.Register<UnitFactory>(Lifetime.Scoped);
            builder.Register<UnitsControllerFactory>(Lifetime.Scoped);
            builder.Register<EntityVisualModelCatalog>(Lifetime.Scoped);
            builder.Register<UnitVariantsCatalog>(Lifetime.Scoped);
        }

        private void RegisterItemFeature(IContainerBuilder builder)
        {
            builder.RegisterInstance(_itemsConfig);

            builder.Register<ItemStackAssembler>(Lifetime.Scoped);
            builder.Register<ItemDefinitionCatalog>(Lifetime.Scoped);
            builder.Register<ItemCommandHandler>(Lifetime.Scoped);

            builder.Register<InventoryModelAssembler>(Lifetime.Scoped);
            builder.Register<EquipmentModelAssembler>(Lifetime.Scoped);
            builder.Register<InventoryEquipmentModuleAssembler>(Lifetime.Scoped);
        }

        private void RegisterStatusEffectFeature(IContainerBuilder builder)
        {
            builder.RegisterInstance(_statusEffectsConfig);

            builder.Register<StatusEffectSystem>(Lifetime.Singleton);
            builder.Register<StatusEffectModuleAssembler>(Lifetime.Singleton);
            builder.Register<StatusEffectAssembler>(Lifetime.Singleton);
            builder.Register<StatusEffectDefinitionCatalog>(Lifetime.Singleton);
        }
    }
}
