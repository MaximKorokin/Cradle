using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class EquipmentSlotView : ContainerSlotView<EquipmentSlotKey>
    {
        [SerializeField]
        private InventorySlotView _inventorySlotView;
        [SerializeField]
        private Image _placeholderImage;
        [SerializeField]
        private Color _blockedSlotColor = new(0.3f, 0.3f, 0.3f, 1f);

        [field: SerializeField]
        public EquipmentSlotType SlotType { get; private set; }

        [Inject]
        public void Construct()
        {
            _inventorySlotView.SetRaycastTarget(false);
        }

        public void Render(ItemStackSnapshot? itemStack, bool isBlocked)
        {
            _inventorySlotView.Render(itemStack, isBlocked ? _blockedSlotColor : Color.white);
            _placeholderImage.enabled = itemStack == null;
        }
    }
}
