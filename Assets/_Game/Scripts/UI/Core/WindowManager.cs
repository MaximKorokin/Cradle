using Assets._Game.Scripts.UI.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public class WindowManager
    {
        private readonly RectTransform _windowsRoot;
        private readonly Stack<UIWindow> _stack = new();

        public WindowManager(UIRootReferences rootReferences)
        {
            _windowsRoot = rootReferences.WindowsRoot;
            UpdateBlocker();
        }

        public T Show<T>(T prefab) where T : UIWindow
        {
            var w = Object.Instantiate(prefab, _windowsRoot);
            _stack.Push(w);
            w.OnShow();
            UpdateBlocker();
            return w;
        }

        public void CloseTop()
        {
            if (_stack.Count == 0) return;

            var w = _stack.Pop();
            w.OnHide();
            Object.Destroy(w.gameObject);
            UpdateBlocker();
        }

        void UpdateBlocker()
        {
            var modal = _stack.Count > 0 && _stack.Peek().IsModal;
        }
    }
}
