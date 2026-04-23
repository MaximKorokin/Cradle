using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
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

        public void Render(ItemStackSnapshot itemStack)
        {
            Clear();

            RenderHeader(itemStack);
            RenderEquippableInfo(itemStack);
            RenderUsableInfo(itemStack);
            RenderCommonInfo(itemStack);

            gameObject.SetActive(true);
        }

        private void RenderHeader(ItemStackSnapshot itemStack)
        {
            _titleText.text = itemStack.Definition.Name;
            _iconView.sprite = itemStack.Definition.Icon;

            if (itemStack.Definition.TryGetTrait<EquippableTrait>(out var equippableTrait))
            {
                _slotText.gameObject.SetActive(true);
                _slotText.text = $"{equippableTrait.Slot}";
            }
        }

        private void RenderEquippableInfo(ItemStackSnapshot itemStack)
        {
            // If the item is not equippable, we don't need to show the equippable info section.
            if (!itemStack.Definition.TryGetTrait<EquippableTrait>(out var equippableTrait)) return;

            if (!TryVisualizeFunctionalTraits(itemStack, ItemTrigger.OnEquipmentChange, _equippableEffectsContainer)) return;

            _equippableInfo.gameObject.SetActive(true);
        }

        private void RenderUsableInfo(ItemStackSnapshot itemStack)
        {
            // If the item is not usable, we don't need to show the usable info section.
            if (!itemStack.Definition.TryGetTrait<UsableTrait>(out var usableTrait)) return;

            if (!TryVisualizeFunctionalTraits(itemStack, ItemTrigger.OnUse, _usableEffectsContainer)) return;

            _usableConsumableInfo.gameObject.SetActive(usableTrait.Consumable);

            _usableCooldownInfo.gameObject.SetActive(usableTrait.Cooldown > 0);
            _usableCooldownText.text = $"{usableTrait.Cooldown}s";

            _usableInfo.gameObject.SetActive(true);
        }

        private void RenderCommonInfo(ItemStackSnapshot itemStack)
        {
            // If the item has no weight and is not stackable, we don't need to show the common info section.
            var hasWeight = itemStack.Definition.Weight > 0;
            var isStackable = itemStack.Definition.MaxAmount > 1;
            if (!hasWeight && !isStackable) return;

            _amountText.text = isStackable ? $"Amount: {itemStack.Amount}" : string.Empty;
            if (hasWeight)
            {
                if (isStackable && itemStack.Amount > 1)
                    _weightText.text = $"Weight: {itemStack.Definition.Weight * itemStack.Amount} ({itemStack.Definition.Weight} each)";
                else
                    _weightText.text = $"Weight: {itemStack.Definition.Weight}";
            }
            else
            {
                _weightText.text = string.Empty;
            }

            _commonInfo.gameObject.SetActive(true);
        }

        private bool TryVisualizeFunctionalTraits(ItemStackSnapshot itemStack, ItemTrigger itemTrigger, RectTransform container)
        {
            // If the item has no traits for the given trigger, we don't need to show anything.
            var traits = itemStack.GetFunctionalTraits<FunctionalItemTraitBase>(itemTrigger).ToArray();
            if (traits.Length == 0) return false;

            // Visualize the traits. If none of the traits have a description, we don't need to show anything.
            var visualizedEffectsCount = 0;
            for (int i = 0; i < traits.Length; i++)
            {
                var description = traits[i].GetDescription();
                if (string.IsNullOrEmpty(description)) continue;
                
                visualizedEffectsCount++;
                var effectView = Instantiate(_equippableEffectTextTemplate, container);
                effectView.text = description;
                effectView.gameObject.SetActive(true);
                _instantiatedEffectViews.Add(effectView.gameObject);
            }

            return visualizedEffectsCount > 0;
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
