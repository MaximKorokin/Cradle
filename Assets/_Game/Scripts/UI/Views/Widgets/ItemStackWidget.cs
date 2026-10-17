using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.Systems.Click;
using Assets._Game.Scripts.UI.Systems.DragDrop;
using Assets._Game.Scripts.UI.Systems.Tooltip;
using Assets.CoreScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public class ItemStackWidget : MonoBehaviour, IClickTarget, IDragDropSource, IDragDropTarget, ITooltipSource
    {
        [SerializeField]
        private bool _canDrag;
        [SerializeField]
        private bool _canDrop;
        [field: SerializeField]
        public ItemStackWidgetType Type { get; private set; }
        [Space]
        [SerializeField]
        private Image _itemImage;
        [SerializeField]
        private Image _highlightImage;
        [SerializeField]
        private TMP_Text _amountText;
        [SerializeField]
        private TMP_Text _enchantText;
        [SerializeField]
        private FillBar _cooldownFillBar;

        private CooldownCounter _itemCooldownCounter;

        public ItemStackPath ItemStackPath { get; private set; }
        public ItemContainerPath ContainerPath => ItemStackPath.ContainerPath;
        public long SlotIndex => ItemStackPath.Slot;

        public bool ContainsData { get; private set; }

        public virtual void Bind(ItemContainerPath itemContainerPath, long itemSlot) => Bind(new(itemContainerPath, itemSlot));

        public virtual void Bind(ItemStackPath itemStackPath)
        {
            ItemStackPath = itemStackPath;
        }

        public void Render(ItemStackSnapshot? itemStack) => Render(itemStack, Color.white);

        public virtual void Render(ItemStackSnapshot? itemStack, Color color)
        {
            ContainsData = itemStack != null;

            _itemCooldownCounter = null;

            if (!ContainsData)
            {
                _itemImage.sprite = null;
                _itemImage.enabled = false;
                _amountText.enabled = false;
                _enchantText.enabled = false;
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

        public virtual bool CanStartDrag()
        {
            return _canDrag && ContainsData;
        }

        public RectTransform CreateDragDropVisual()
        {
            var image = new GameObject().AddComponent<Image>();
            image.sprite = _itemImage.sprite;
            image.color = new(0.8f, 0.8f, 0.8f);
            image.rectTransform.sizeDelta = _itemImage.rectTransform.rect.size;

            return image.transform as RectTransform;
        }

        public bool CanDrop() => _canDrop;

        public void SetDragDropHighlight(bool highlighted)
        {
            _highlightImage.enabled = highlighted;
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

    public enum ItemStackWidgetType
    {
        None = 0,

        Definition = 100,
        Reference = 110,

        Inventory = 200,
        Equipment = 210,
        Shop = 220,
    }
}
