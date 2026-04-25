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

        private List<GameObject> _instantiatedEffectViews = new();

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

            if (!TryVisualizeDescription(itemStack.EquippableEffectsDescription, _equippableEffectTextTemplate, _equippableEffectsContainer)) return;

            _equippableInfo.gameObject.SetActive(true);
        }

        private void RenderUsableInfo(ItemStackDisplayData itemStack)
        {
            // If the item is not usable, we don't need to show the usable info section.
            if (!itemStack.IsUsable) return;

            if (!TryVisualizeDescription(itemStack.UsableEffectsDescription, _usableEffectTextTemplate, _usableEffectsContainer)) return;

            _usableConsumableInfo.gameObject.SetActive(itemStack.IsConsumable);

            _usableCooldownInfo.gameObject.SetActive(!string.IsNullOrEmpty(itemStack.UsableCooldownDescription));
            _usableCooldownText.text = itemStack.UsableCooldownDescription;

            _usableInfo.gameObject.SetActive(true);
        }

        private void RenderCommonInfo(ItemStackDisplayData itemStack)
        {
            // If the item has no weight and is not stackable, we don't need to show the common info section.
            var hasWeight = !string.IsNullOrEmpty(itemStack.WeightDescription);
            var isStackable = !string.IsNullOrEmpty(itemStack.AmountDescription);
            if (!hasWeight && !isStackable) return;

            _amountText.gameObject.SetActive(isStackable);
            _amountText.text = itemStack.AmountDescription;

            _weightText.gameObject.SetActive(hasWeight);
            _weightText.text = itemStack.WeightDescription;

            _commonInfo.gameObject.SetActive(true);
        }

        private bool TryVisualizeDescription(string description, TMP_Text template, RectTransform container)
        {
            if (string.IsNullOrEmpty(description)) return false;

            var effectView = Instantiate(template, container);
            effectView.text = description;
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
        }
    }
}
