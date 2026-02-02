using Assets._Game.Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class InventorySlotView : MonoBehaviour
    {
        [SerializeField]
        private Image _itemImage;
        [SerializeField]
        private TMP_Text _amountText;

        public void Render(ItemStack itemStack)
        {
            if (itemStack == null)
            {
                _amountText.enabled = false;
                _itemImage.enabled = false;
                return;
            }

            _itemImage.enabled = true;
            _itemImage.sprite = itemStack.Definition.Icon;

            if (itemStack.Definition.MaxAmount > 1)
            {
                _amountText.text = itemStack.Amount.ToString();
                _amountText.enabled = true;
            }
            else
            {
                _amountText.enabled = false;
            }
        }
    }
}
