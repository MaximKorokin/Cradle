using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._Game.Scripts.UI.Views
{
    public abstract class ContainerSlotView<T> : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public T SlotIndex { get; private set; }

        public event Action<T> PointerDown;
        public event Action<T> PointerUp;

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            PointerDown?.Invoke(SlotIndex);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            PointerUp?.Invoke(SlotIndex);
        }

        public virtual void Bind(T slotIndex)
        {
            SlotIndex = slotIndex;
        }
    }
}
