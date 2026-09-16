using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class EquipmentSlotView : MonoBehaviour
    {
        [SerializeField]
        private InventorySlotView _inventorySlotView;
        [SerializeField]
        private Image _placeholderImage;
        [SerializeField]
        private Color _blockedSlotColor = new(0.3f, 0.3f, 0.3f, 1f);

        [field: SerializeField]
        [field: Tooltip("Used to bind Model slot to UI")]
        public EquipmentSlotType SlotType { get; private set; }

        public InventorySlotView SlotView => _inventorySlotView;

        public void Render(ItemStackSnapshot? itemStack, bool isBlocked)
        {
            _inventorySlotView.Render(itemStack, isBlocked ? _blockedSlotColor : Color.white);
            _placeholderImage.enabled = itemStack == null;
        }
    }
}
