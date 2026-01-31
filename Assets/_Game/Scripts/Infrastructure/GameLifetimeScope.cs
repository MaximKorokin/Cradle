using Assets._Game.Scripts.Entities;
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

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameBootstrap>();

            builder.RegisterInstance(_entityUnitsManager);
            builder.RegisterInstance(_entityVisualModelsManager);

            builder.Register<EntityAssembler>(Lifetime.Scoped);

            RegisterSavesFeature(builder);
            RegisterItemsFeature(builder);
            RegisterInventoryFeature(builder);
        }

        private void RegisterSavesFeature(IContainerBuilder builder)
        {
            builder.Register<PlayerPrefsSavesStorage>(Lifetime.Scoped);
            builder.Register<JsonSaveSerializer>(Lifetime.Scoped);
        }

        private void RegisterItemsFeature(IContainerBuilder builder)
        {
            builder.Register<ItemCatalog>(Lifetime.Scoped);
        }

        private void RegisterInventoryFeature(IContainerBuilder builder)
        {
            builder.Register<InventoryModel>(Lifetime.Scoped);
            builder.Register<EquipmentModel>(Lifetime.Scoped);
            builder.Register<InventoryEquipmentController>(Lifetime.Scoped);

            builder.Register<ItemContainerSaveRepository>(Lifetime.Scoped);
        }
    }
}