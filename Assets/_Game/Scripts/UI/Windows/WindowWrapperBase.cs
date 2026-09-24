using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Systems;
using System;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.UI.Windows
{
    public abstract class WindowWrapperBase : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _windowParent;

        private IGlobalEventBus _globalEventBus;

        private UIWindowBase _window;

        [Inject]
        private void Construct(IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;
        }

        public void SetWindow(UIWindowBase window)
        {
            _window = window;
            window.transform.SetParent(_windowParent, false);
        }

        public void RequestClose()
        {
            _globalEventBus.Publish(new WindowCloseRequest(_window));
        }
    }
}
