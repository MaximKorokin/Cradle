using Assets._Game.Scripts.Items;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._Game.Scripts.UI.Views
{
    public abstract class ContainerSlotView<T> : MonoBehaviour, IPointerClickHandler where T : IContainerSlot
    {
        public ItemContainerPath ContainerPath { get; private set; }
        public T SlotIndex { get; private set; }

        public event Action<T> PointerClick;

        public virtual void Bind(ItemContainerPath containerPath, T slotIndex)
        {
            ContainerPath = containerPath;
            SlotIndex = slotIndex;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PointerClick?.Invoke(SlotIndex);
        }
    }
}
