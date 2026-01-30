using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Items.Equipment;
using Assets._Game.Scripts.Entities.Items.Inventory;
using Assets._Game.Scripts.ScriptableObjectManagers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField]
    private EntityUnitVariantsManager _entityUnitsManager;
    [SerializeField]
    private EntityVisualModelsManager _entityVisualModelsManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<GameController>();

        builder.RegisterInstance(_entityUnitsManager);
        builder.RegisterInstance(_entityVisualModelsManager);

        builder.Register<EntityBuilder>(Lifetime.Scoped);

        RegisterInventoryFeature(builder);
    }

    private void RegisterInventoryFeature(IContainerBuilder builder)
    {
        builder.Register<InventoryModel>(Lifetime.Scoped);

        builder.Register<EquipmentModel>(Lifetime.Scoped);
    }
}
