using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._Game.Scripts.UI.Views
{
    public abstract class ContainerSlotView<T> : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private T _slotIndex;

        public event Action<T> PointerDown;
        public event Action<T> PointerUp;

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown?.Invoke(_slotIndex);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerUp?.Invoke(_slotIndex);
        }

        public void Bind(T slotIndex)
        {
            _slotIndex = slotIndex;
        }
    }
}
