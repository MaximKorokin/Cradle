using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class WindowWrapper : WindowWrapperBase
    {
        [SerializeField]
        private Button _closeButton;

        private void Awake()
        {
            _closeButton.onClick.AddListener(RequestClose);
        }
    }
}
