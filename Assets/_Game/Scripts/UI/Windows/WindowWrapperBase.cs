using System;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public abstract class WindowWrapperBase : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _windowParent;

        private UIWindowBase _window;

        public event Action<UIWindowBase> WindowCloseRequested;

        public void SetWindow(UIWindowBase window)
        {
            _window = window;
            window.transform.SetParent(_windowParent, false);
        }

        public void RequestClose()
        {
            WindowCloseRequested?.Invoke(_window);
        }
    }
}
