using Assets._Game.Scripts.Items;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public abstract class ContainerSlotWidget : MonoBehaviour
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
