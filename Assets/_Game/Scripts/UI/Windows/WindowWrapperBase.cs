using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Systems;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.UI.Windows
{
    public abstract class WindowWrapperBase : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _windowParent;

        private IGlobalEventBus _globalEventBus;

        public WindowId WindowId { get; private set; }
        public UIWindowBase Window { get; private set; }

        [Inject]
        private void Construct(IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;
        }

        public void SetWindow(WindowId windowId, UIWindowBase window)
        {
            WindowId = windowId;
            Window = window;
            window.transform.SetParent(_windowParent, false);
        }

        public void RequestClose()
        {
            _globalEventBus.Publish(new WindowCloseRequest(Window));
        }
    }
}
