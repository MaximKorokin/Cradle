using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public class AppLifecycleBridge : MonoBehaviour
    {
        IAppLifecycleHandler _handler;

        [Inject]
        public void Construct(IAppLifecycleHandler handler)
        {
            _handler = handler;
        }

        private void Update()
        {
            _handler.OnUpdate();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                _handler.OnPause();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                _handler.OnFocusLost();
            }
        }

        private void OnApplicationQuit()
        {
            _handler.OnQuit();
        }
    }
}
