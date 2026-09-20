using System;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows.Modal
{
    public sealed class ModalWrapper : WindowWrapperBase
    {
        [SerializeField]
        private ModalOverlay _overlay;

        private void Awake()
        {
            _overlay.PointerDown += RequestClose;
        }
    }
}
