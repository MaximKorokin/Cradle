using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Infrastructure.Persistence.Codecs;
using Assets._Game.Scripts.Items;
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

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameBootstrap>();

            builder.RegisterInstance(_newGameDefinition);

            builder.Register<PlayerContext>(Lifetime.Singleton);

            RegisterSavesFeature(builder);
            RegisterEntityFeature(builder);
            RegisterInventoryFeature(builder);
            RegisterItemFeature(builder);
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

            builder.Register<StatsModuleAssembler>(Lifetime.Scoped);
            builder.Register<StatsControllerAssembler>(Lifetime.Scoped);

            builder.Register<EntityAppearanceModuleFactory>(Lifetime.Scoped);
            builder.Register<EntityUnitFactory>(Lifetime.Scoped);
            builder.Register<EntityUnitsControllerFactory>(Lifetime.Scoped);
            builder.Register<EntityVisualModelCatalog>(Lifetime.Scoped);
            builder.Register<EntityUnitVariantsCatalog>(Lifetime.Scoped);
        }

        private void RegisterInventoryFeature(IContainerBuilder builder)
        {
            builder.Register<InventoryModelAssembler>(Lifetime.Scoped);
            builder.Register<EquipmentModelAssembler>(Lifetime.Scoped);
            builder.Register<EntityInventoryEquipmentModuleAssembler>(Lifetime.Scoped);
        }

        private void RegisterItemFeature(IContainerBuilder builder)
        {
            builder.Register<ItemStackAssembler>(Lifetime.Scoped);
            builder.Register<ItemDefinitionCatalog>(Lifetime.Scoped);
        }
    }
}
