using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.UI.Systems.Click;
using Assets._Game.Scripts.UI.Systems.DragDrop;
using Assets._Game.Scripts.UI.Systems.Tooltip;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public abstract class ContainerSlotWidget : MonoBehaviour, IClickTarget, IDragDropSource, ITooltipSource
    {
        [SerializeField]
        private Image _itemImage;

        public ItemContainerPath ContainerPath { get; private set; }
        public long SlotIndex { get; private set; }

        public bool ContainsData { get; private set; }

        public virtual void Bind(ItemContainerPath containerPath, long slotIndex)
        {
            ContainerPath = containerPath;
            SlotIndex = slotIndex;
        }

        public virtual void Render(ItemStackSnapshot? itemStack, Color color)
        {
            ContainsData = itemStack != null;
            if (!ContainsData)
            {
                _itemImage.sprite = null;
                _itemImage.enabled = false;
                return;
            }

            _itemImage.enabled = true;
            _itemImage.sprite = itemStack.Value.Definition.Icon;
            _itemImage.color = color;

        }

        public bool CanStartDrag()
        {
            return ContainsData;
        }

        public RectTransform CreateDragDropVisual()
        {
            var image = new GameObject().AddComponent<Image>();
            image.sprite = _itemImage.sprite;
            image.color = new(0.8f, 0.8f, 0.8f);
            image.rectTransform.sizeDelta = _itemImage.rectTransform.rect.size;

            return image.transform as RectTransform;
        }
    }
}
