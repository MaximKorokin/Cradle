using Assets._Game.Scripts.Entities;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure
{
    public class GameBootstrap : IStartable, ITickable
    {
        private readonly EntityAssembler _entityBuilder;

        public GameBootstrap(EntityAssembler entityBuilder)
        {
            _entityBuilder = entityBuilder;
        }

        public void Start()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            Application.targetFrameRate = 60;

            entity1 = _entityBuilder.Assemble("Humanoid");
            entity2 = _entityBuilder.Assemble("Quadruped");

            SceneManager.LoadSceneAsync("UIRoot", LoadSceneMode.Additive);
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
}