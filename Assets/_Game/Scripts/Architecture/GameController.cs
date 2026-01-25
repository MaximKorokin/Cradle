using Assets._Game.Scripts.Entities;
using System.Globalization;
using UnityEngine;
using VContainer.Unity;

public class GameController : IStartable
{
    private EntityBuilder _entityBuilder;

    public GameController(EntityBuilder entityBuilder)
    {
        _entityBuilder = entityBuilder;
    }

    public void Start()
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
        Application.targetFrameRate = 60;

        var entity = _entityBuilder.Build("Humanoid");
        entity.UnitsController.PlayAnimationByTrigger("ToIdle");
    }
}
