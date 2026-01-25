using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.ScriptableObjects;
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
    }
}
