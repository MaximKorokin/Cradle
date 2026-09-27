using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.UI.Systems.Click;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public abstract class ContainerSlotWidget : MonoBehaviour, IClickTarget
    {
        public ItemContainerPath ContainerPath { get; private set; }
        public long SlotIndex { get; private set; }

        public virtual void Bind(ItemContainerPath containerPath, long slotIndex)
        {
            ContainerPath = containerPath;
            SlotIndex = slotIndex;
        }
    }
}
