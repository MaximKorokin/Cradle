using Assets._Game.Scripts.Items;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._Game.Scripts.UI.Views
{
    public abstract class ContainerSlotView : MonoBehaviour, IPointerClickHandler
    {
        public ItemContainerPath ContainerPath { get; private set; }
        public long SlotIndex { get; private set; }

        public event Action<long> PointerClick;

        public virtual void Bind(ItemContainerPath containerPath, long slotIndex)
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
