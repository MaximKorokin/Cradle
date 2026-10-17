using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public sealed class EquipmentSlotWidget : MonoBehaviour
    {
        [SerializeField]
        private ItemStackWidget _itemStackWidget;
        [SerializeField]
        private Image _placeholderImage;
        [SerializeField]
        private Color _blockedSlotColor = new(0.3f, 0.3f, 0.3f, 1f);

        [field: SerializeField]
        [field: Tooltip("Used to bind Model slot to UI")]
        public EquipmentSlotType SlotType { get; private set; }

        public ItemStackWidget SlotView => _itemStackWidget;
        public long? BlockingSlotIndex { get; private set; }

        public void Render(ItemStackSnapshot? itemStack, bool isBlocked)
        {
            _itemStackWidget.Render(itemStack, isBlocked ? _blockedSlotColor : Color.white);
            _placeholderImage.enabled = itemStack == null;
        }

        public void Bind(ItemContainerPath itemContainerPath, long itemSlot, long? blockingSlotIndex)
        {
            BlockingSlotIndex = blockingSlotIndex;
            _itemStackWidget.Bind(itemContainerPath, itemSlot);
        }
    }
}
