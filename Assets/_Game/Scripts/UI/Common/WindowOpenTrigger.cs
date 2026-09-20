using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Systems;
using Assets._Game.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Assets._Game.Scripts.UI.Common
{
    [RequireComponent(typeof(Button))]
    public sealed class WindowOpenTrigger : MonoBehaviour
    {
        [SerializeField] private WindowId _windowId;

        private IGlobalEventBus _globalEventBus;

        private WindowControllerArgumentsProvider _controllerArgumentsProvider;

        [Inject]
        private void Construct(
            IGlobalEventBus globalEventBus,
            WindowControllerArgumentsProvider controllerArgumentsProvider)
        {
            _controllerArgumentsProvider = controllerArgumentsProvider;
            _globalEventBus = globalEventBus;
        }

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            var arguments = _controllerArgumentsProvider.GetPlayerArguments(_windowId);
            _globalEventBus.Publish(new WindowToggleRequest(_windowId, arguments));
        }
    }
}
