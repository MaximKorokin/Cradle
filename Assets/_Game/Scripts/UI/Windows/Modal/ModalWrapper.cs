using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.UI.Windows.Modal
{
    public sealed class ModalWrapper : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _windowParent;
        [SerializeField]
        private ModalOverlay _overlay;

        private WindowManager _windowManager;

        private UIWindowBase _window;

        [Inject]
        private void Construct(WindowManager windowManager)
        {
            _windowManager = windowManager;

            _overlay.PointerDown += Close;
        }

        public void SetWindow(UIWindowBase window)
        {
            _window = window;
            window.transform.SetParent(_windowParent, false);
        }

        public void Close()
        {
            _windowManager.CloseWindow(_window);
        }
    }
}
