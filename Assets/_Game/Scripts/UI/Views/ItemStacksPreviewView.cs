using Assets._Game.Scripts.UI.DataFormatters;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class ItemStacksPreviewView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _titleText;
        [SerializeField]
        private TMP_Text _slotText;
        [SerializeField]
        private Image _iconView;
        [Space]
        [SerializeField]
        private RectTransform _equippableInfo;
        [SerializeField]
        private RectTransform _equippableEffectsContainer;
        [SerializeField]
        private TMP_Text _equippableEffectTextTemplate;
        [Space]
        [SerializeField]
        private RectTransform _usableInfo;
        [SerializeField]
        private RectTransform _usableEffectsContainer;
        [SerializeField]
        private TMP_Text _usableEffectTextTemplate;
        [SerializeField]
        private RectTransform _usableConsumableInfo;
        [SerializeField]
        private RectTransform _usableCooldownInfo;
        [SerializeField]
        private TMP_Text _usableCooldownText;
        [Space]
        [SerializeField]
        private RectTransform _commonInfo;
        [SerializeField]
        private TMP_Text _amountText;
        [SerializeField]
        private TMP_Text _weightText;
        [SerializeField]
        private TMP_Text _priceText;
        [Space]
        [SerializeField]
        private RectTransform _descriptionInfo;
        [SerializeField]
        private TMP_Text _descriptionText;

        private readonly List<GameObject> _instantiatedEffectViews = new();

        private void Awake()
        {
            _equippableEffectTextTemplate.gameObject.SetActive(false);
            _usableEffectTextTemplate.gameObject.SetActive(false);
        }

        public void Render(ItemStackDisplayData itemStack)
        {
            Clear();

            RenderHeader(itemStack);
            RenderEquippableInfo(itemStack);
            RenderUsableInfo(itemStack);
            RenderCommonInfo(itemStack);
            RenderDescription(itemStack);

            gameObject.SetActive(true);
        }

        private void RenderHeader(ItemStackDisplayData itemStack)
        {
            _titleText.text = itemStack.Name;
            _iconView.sprite = itemStack.Icon;

            if (itemStack.IsEquippable)
            {
                _slotText.gameObject.SetActive(true);
                _slotText.text = itemStack.EquipmentSlotName;
            }
        }

        private void RenderEquippableInfo(ItemStackDisplayData itemStack)
        {
            // If the item is not equippable, we don't need to show the equippable info section.
            if (!itemStack.IsEquippable) return;

            if (!TryVisualizeText(itemStack.EquippableEffectsText, _equippableEffectTextTemplate, _equippableEffectsContainer)) return;

            _equippableInfo.gameObject.SetActive(true);
        }

        private void RenderUsableInfo(ItemStackDisplayData itemStack)
        {
            // If the item is not usable, we don't need to show the usable info section.
            if (!itemStack.IsUsable) return;

            if (!TryVisualizeText(itemStack.UsableEffectsText, _usableEffectTextTemplate, _usableEffectsContainer)) return;

            _usableConsumableInfo.gameObject.SetActive(itemStack.IsConsumable);

            _usableCooldownInfo.gameObject.SetActive(!string.IsNullOrEmpty(itemStack.UsableCooldownText));
            _usableCooldownText.text = itemStack.UsableCooldownText;

            _usableInfo.gameObject.SetActive(true);
        }

        private void RenderDescription(ItemStackDisplayData itemStack)
        {
            if (_descriptionText == null) return;

            var hasDescription = !string.IsNullOrEmpty(itemStack.Description);
            _descriptionText.gameObject.SetActive(hasDescription);
            _descriptionText.text = itemStack.Description;

            _descriptionInfo.gameObject.SetActive(hasDescription);
        }

        private void RenderCommonInfo(ItemStackDisplayData itemStack)
        {
            var hasWeight = !string.IsNullOrEmpty(itemStack.WeightText);
            var isStackable = !string.IsNullOrEmpty(itemStack.AmountText);
            var hasPrice = !string.IsNullOrEmpty(itemStack.PriceText);
            if (!hasWeight && !isStackable && !hasPrice) return;

            _amountText.gameObject.SetActive(isStackable);
            _amountText.text = itemStack.AmountText;

            _weightText.gameObject.SetActive(hasWeight);
            _weightText.text = itemStack.WeightText;

            _priceText.gameObject.SetActive(hasPrice);
            _priceText.text = itemStack.PriceText;

            _commonInfo.gameObject.SetActive(true);
        }

        private bool TryVisualizeText(string text, TMP_Text template, RectTransform container)
        {
            if (string.IsNullOrEmpty(text)) return false;

            var effectView = Instantiate(template, container);
            effectView.text = text;
            effectView.gameObject.SetActive(true);

            _instantiatedEffectViews.Add(effectView.gameObject);

            return true;
        }

        public void Clear()
        {
            gameObject.SetActive(false);

            for (int i = 0; i < _instantiatedEffectViews.Count; i++)
            {
                Destroy(_instantiatedEffectViews[i]);
            }
            _instantiatedEffectViews.Clear();

            _titleText.text = string.Empty;
            _slotText.text = string.Empty;
            _iconView.sprite = null;

            _equippableInfo.gameObject.SetActive(false);

            _usableInfo.gameObject.SetActive(false);

            _commonInfo.gameObject.SetActive(false);

            _descriptionInfo.gameObject.SetActive(false);
        }
    }
}
