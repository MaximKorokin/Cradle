using Assets._Game.Scripts.Entities;
using System.Globalization;
using UnityEngine;
using VContainer.Unity;

public class GameController : IStartable, ITickable
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

        entity1 = _entityBuilder.Build("Humanoid");
        entity2 = _entityBuilder.Build("Quadruped");

        //entity.UnitsController.AnimatorController.SetAnimation(EntityAnimationClipName.ActionHands, new());
    }

    Entity entity1;
    Entity entity2;
    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            entity1.UnitsController.UpdateOrderInLayer();
        }
    }
}
