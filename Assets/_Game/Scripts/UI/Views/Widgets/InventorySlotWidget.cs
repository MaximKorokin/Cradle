using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.Systems.DragDrop;
using Assets.CoreScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public sealed class InventorySlotWidget : ContainerSlotWidget, IDragDropTarget
    {
        [SerializeField]
        private Image _highlightImage;
        [SerializeField]
        private TMP_Text _amountText;
        [SerializeField]
        private TMP_Text _enchantText;
        [SerializeField]
        private FillBar _cooldownFillBar;

        private CooldownCounter _itemCooldownCounter;

        public void Render(ItemStackSnapshot? itemStack) => Render(itemStack, Color.white);

        public override void Render(ItemStackSnapshot? itemStack, Color color)
        {
            base.Render(itemStack, color);

            _itemCooldownCounter = null;
            if (!ContainsData)
            {
                _amountText.enabled = false;
                _enchantText.enabled = false;
                _cooldownFillBar.gameObject.SetActive(false);
                return;
            }

            if (itemStack.Value.Definition.MaxAmount > 1)
            {
                _amountText.text = itemStack.Value.Amount.ToString();
                _amountText.enabled = true;
            }
            else
            {
                _amountText.enabled = false;
            }

            if (itemStack.Value.InstanceData?.TryGet<EnchantInstanceData>(out var enchantData) == true && enchantData.Level > 0)
            {
                _enchantText.text = $"+{enchantData.Level}";
                _enchantText.enabled = true;
            }
            else
            {
                _enchantText.enabled = false;
            }

            _amountText.color = new() { r = _amountText.color.r, g = _amountText.color.g, b = _amountText.color.b, a = color.a };
            _enchantText.color = new() { r = _enchantText.color.r, g = _enchantText.color.g, b = _enchantText.color.b, a = color.a };

            // If the item has cooldown data, we want to show the cooldown fill bar and update it in Update()
            _itemCooldownCounter =
                itemStack.Value.InstanceData?.TryGet<CooldownInstanceData>(out var cooldownData) == true
                    ? cooldownData.CooldownCounter
                    : null;
            _cooldownFillBar.gameObject.SetActive(_itemCooldownCounter != null && _itemCooldownCounter.Cooldown > 0);
        }

        private void Update()
        {
            if (_itemCooldownCounter == null)
            {
                return;
            }

            if (_itemCooldownCounter.TimeSinceReset >= _itemCooldownCounter.Cooldown && _cooldownFillBar.CurrentFillRatio != 0)
            {
                _cooldownFillBar.SetFillRatio(0);
                return;
            }

            if (_itemCooldownCounter.Cooldown > 0 && _itemCooldownCounter.TimeSinceReset <= _itemCooldownCounter.Cooldown)
            {
                _cooldownFillBar.SetFillRatio(1 - _itemCooldownCounter.TimeSinceReset / _itemCooldownCounter.Cooldown);
            }
        }

        public void SetDragDropHighlight(bool highlighted)
        {
            _highlightImage.enabled = highlighted;
        }
    }
}
