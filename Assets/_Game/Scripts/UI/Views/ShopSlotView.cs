using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.Items.Traits;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class ShopSlotView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TMP_Text _amountText;
        [SerializeField]
        private TMP_Text _priceText;

        private ShopSlot _slot;

        public event Action<ShopSlot> PointerClick;

        public void Bind(ShopSlot slot)
        {
            _slot = slot;
        }

        public void Render(ItemStackSnapshot? snapshot)
        {
            if (!snapshot.HasValue)
            {
                _icon.enabled = false;
                _amountText.text = string.Empty;
                _priceText.text = string.Empty;
                return;
            }

            _icon.enabled = true;
            _icon.sprite = snapshot.Value.Definition.Icon;
            _amountText.text = snapshot.Value.Amount > 1 ? snapshot.Value.Amount.ToString() : string.Empty;

            // Display price if item has PriceTrait
            if (snapshot.Value.Definition.TryGetTrait<PriceTrait>(out var priceTrait))
            {
                _priceText.text = $"{priceTrait.BasePrice}g";
            }
            else
            {
                _priceText.text = string.Empty;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PointerClick?.Invoke(_slot);
        }
    }
}
