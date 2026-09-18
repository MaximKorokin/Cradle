using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class WindowWrapper : MonoBehaviour
    {
        [SerializeField]
        private Button _closeButton;
        [SerializeField]
        private RectTransform _windowParent;

        private WindowManager _windowManager;

        private UIWindowBase _window;

        [Inject]
        private void Construct(WindowManager windowManager)
        {
            _windowManager = windowManager;

            _closeButton.onClick.AddListener(Close);
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
