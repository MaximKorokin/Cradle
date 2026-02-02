using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.ScriptableObjectManagers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private EntityUnitVariantsManager _entityUnitsManager;
        [SerializeField]
        private EntityVisualModelsManager _entityVisualModelsManager;
        [SerializeField]
        private NewGameDefinition _newGameDefinition;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameBootstrap>();

            builder.RegisterInstance(_entityUnitsManager);
            builder.RegisterInstance(_entityVisualModelsManager);
            builder.RegisterInstance(_newGameDefinition);

            builder.Register<GameContext>(Lifetime.Singleton);

            RegisterSavesFeature(builder);
            RegisterEntityFeature(builder);
            RegisterInventoryFeature(builder);
            RegisterItemFeature(builder);
        }

        private void RegisterSavesFeature(IContainerBuilder builder)
        {
            builder.Register<GameSaveRepository>(Lifetime.Scoped);
            builder.Register<ISaveStorage, PlayerPrefsSavesStorage>(Lifetime.Scoped);
            builder.Register<ISaveSerializer, JsonSaveSerializer>(Lifetime.Scoped);
        }

        private void RegisterEntityFeature(IContainerBuilder builder)
        {
            builder.Register<EntityRepository>(Lifetime.Scoped);
            builder.Register<EntityAssembler>(Lifetime.Scoped);
            builder.Register<EntityDefinitionCatalog>(Lifetime.Scoped);
        }

        private void RegisterInventoryFeature(IContainerBuilder builder)
        {
            builder.Register<InventoryModelAssembler>(Lifetime.Scoped);
            builder.Register<EquipmentModelAssembler>(Lifetime.Scoped);
            builder.Register<InventoryEquipmentControllerAssembler>(Lifetime.Scoped);
        }

        private void RegisterItemFeature(IContainerBuilder builder)
        {
            builder.Register<ItemStackAssembler>(Lifetime.Scoped);
            builder.Register<ItemDefinitionCatalog>(Lifetime.Scoped);
        }
    }
}