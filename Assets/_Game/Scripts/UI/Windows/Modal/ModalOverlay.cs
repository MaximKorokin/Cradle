using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._Game.Scripts.UI.Windows.Modal
{
    public sealed class ModalOverlay : MonoBehaviour, IPointerDownHandler
    {
        public event Action PointerDown;

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown?.Invoke();
        }
    }
}
