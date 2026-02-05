using Assets._Game.Scripts.Items;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class InventorySlotView : ContainerSlotView<int>
    {
        [SerializeField]
        private Image _itemImage;
        [SerializeField]
        private TMP_Text _amountText;

        public void Render(ItemStackSnapshot? itemStack)
        {
            if (itemStack == null)
            {
                _amountText.enabled = false;
                _itemImage.enabled = false;
                return;
            }

            _itemImage.enabled = true;
            _itemImage.sprite = itemStack.Value.Definition.Icon;

            if (itemStack.Value.Definition.MaxAmount > 1)
            {
                _amountText.text = itemStack.Value.Amount.ToString();
                _amountText.enabled = true;
            }
            else
            {
                _amountText.enabled = false;
            }
        }

        public void SetRaycastTarget(bool isTarget)
        {
            foreach (var graphic in GetComponents<Graphic>().Union(GetComponentsInChildren<Graphic>()))
            {
                graphic.raycastTarget = isTarget;
            }
        }
    }
}
