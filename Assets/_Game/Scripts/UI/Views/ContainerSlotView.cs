using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._Game.Scripts.UI.Views
{
    public abstract class ContainerSlotView<T> : MonoBehaviour, IPointerClickHandler
    {
        public T SlotIndex { get; private set; }

        public event Action<T> PointerClick;

        public virtual void Bind(T slotIndex)
        {
            SlotIndex = slotIndex;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PointerClick?.Invoke(SlotIndex);
        }
    }
}
