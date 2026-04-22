using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.Common;
using Assets.CoreScripts;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class InventorySlotView : ContainerSlotView<InventorySlot>
    {
        [SerializeField]
        private Image _itemImage;
        [SerializeField]
        private TMP_Text _amountText;
        [SerializeField]
        private FillBar _cooldownFillBar;

        private CooldownCounter _itemCooldownCounter;

        public void Render(ItemStackSnapshot? itemStack) => Render(itemStack, Color.white);

        public void Render(ItemStackSnapshot? itemStack, Color color)
        {
            _itemCooldownCounter = null;
            if (itemStack == null)
            {
                _amountText.enabled = false;
                _itemImage.enabled = false;
                _cooldownFillBar.gameObject.SetActive(false);
                return;
            }

            _itemImage.enabled = true;
            _itemImage.sprite = itemStack.Value.Definition.Icon;
            _itemImage.color = color;

            if (itemStack.Value.Definition.MaxAmount > 1)
            {
                _amountText.text = itemStack.Value.Amount.ToString();
                _amountText.enabled = true;
            }
            else
            {
                _amountText.enabled = false;
            }

            _amountText.color = new() { r = _amountText.color.r, g = _amountText.color.g, b = _amountText.color.b, a = color.a };

            // If the item has cooldown data, we want to show the cooldown fill bar and update it in Update()
            _itemCooldownCounter = (itemStack.Value.InstanceData as CooldownInstanceData)?.CooldownCounter;
            _cooldownFillBar.gameObject.SetActive(_itemCooldownCounter != null && _itemCooldownCounter.Cooldown > 0);
        }

        public void SetRaycastTarget(bool isTarget)
        {
            foreach (var graphic in GetComponents<Graphic>().Union(GetComponentsInChildren<Graphic>()))
            {
                graphic.raycastTarget = isTarget;
            }
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
    }
}
