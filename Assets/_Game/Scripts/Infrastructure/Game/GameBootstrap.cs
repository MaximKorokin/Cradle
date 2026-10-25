using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.UI.Core;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public class GameBootstrap : IStartable
    {
        private readonly IGlobalEventBus _globalEventBus;

        public GameBootstrap(IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;

            _globalEventBus.SubscribeOnce<UIInitializedEvent>(OnUIInitialized);
        }

        public void Start()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            Application.targetFrameRate = 60;

            SceneManager.LoadScene("UIRoot", LoadSceneMode.Additive);
        }

        private void OnUIInitialized(UIInitializedEvent e)
        {
            _globalEventBus.Publish(new GameInitializedEvent());
            _globalEventBus.Publish(new LoadGameRequest(""));
        }
    }

    public readonly struct GameInitializedEvent : IGlobalEvent
    {
    }
}
