using Assets._Game.Scripts.Items;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class InventorySlotView : MonoBehaviour
    {
        [SerializeField]
        private Image _itemImage;

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
