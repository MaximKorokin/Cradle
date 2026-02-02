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
        private Image _itemImage;
        [SerializeField]
        private Image _placeholderImage;

        [field: SerializeField]
        public EquipmentSlotType SlotType { get; private set; }

        [Inject]
        public void Construct()
        {
            // default slot icons
        }

        public void Render(ItemStack itemStack)
        {
            if (itemStack == null)
            {
                _itemImage.enabled = false;
                return;
            }
            _itemImage.enabled = true;
            _itemImage.sprite = itemStack.Definition.Icon;
        }
    }
}
