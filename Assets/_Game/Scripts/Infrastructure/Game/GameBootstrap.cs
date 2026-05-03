using Assets._Game.Scripts.Infrastructure.Persistence;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public class GameBootstrap : IStartable
    {
        private readonly SaveService _saveService;

        public GameBootstrap(SaveService saveService)
        {
            _saveService = saveService;
        }

        public void Start()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            Application.targetFrameRate = 60;

            SceneManager.LoadScene("UIRoot", LoadSceneMode.Additive);

            _saveService.LoadGame();
        }
    }
}
