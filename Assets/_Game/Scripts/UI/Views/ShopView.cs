using Assets._Game.Scripts.Items.Shop;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class ShopView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _shopSlotsParent;
        [SerializeField]
        private ShopSlotView _shopSlotTemplate;
        [Space]
        [SerializeField]
        private TMP_Text _shopNameText;
        [SerializeField]
        private TMP_Text _goldText;

        private readonly List<ShopSlotView> _slots = new();

        private ShopModel _shopModel;

        public event Action<ShopSlot> SlotClick;

        public void Render(ShopModel shopModel, string shopName, int playerGold)
        {
            _shopSlotTemplate.gameObject.SetActive(false);

            _shopModel = shopModel;
            _shopNameText.text = shopName;
            _goldText.text = $"Gold: {playerGold}";

            foreach (var slot in _slots)
            {
                slot.gameObject.SetActive(false);
            }

            foreach (var (shopSlot, stack) in _shopModel.Enumerate())
            {
                if (_slots.Count > shopSlot.Index)
                {
                    var slot = _slots[shopSlot.Index];
                    slot.Render(stack);
                    slot.gameObject.SetActive(true);
                    continue;
                }

                var newSlot = Instantiate(_shopSlotTemplate, _shopSlotsParent);
                newSlot.Bind(shopSlot);
                newSlot.PointerClick += OnSlotPointerClick;
                _slots.Add(newSlot);
                newSlot.gameObject.SetActive(true);
                newSlot.Render(stack);
            }
        }

        public void Unbind()
        {
            _shopModel = null;
        }

        private void OnSlotPointerClick(ShopSlot slotIndex)
        {
            SlotClick?.Invoke(slotIndex);
        }
    }
}
