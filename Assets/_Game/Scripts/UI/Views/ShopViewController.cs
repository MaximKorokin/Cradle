using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.Views;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ShopViewController
    {
        private ShopView _view;
        private ShopModel _shopModel;
        private string _shopName;
        private float _buyCoefficient;
        private float _sellCoefficient;

        public event Action<ShopSlot> SlotClick;

        public void Initialize(ShopView view, ShopModel shopModel, string shopName, float buyCoefficient, float sellCoefficient)
        {
            _view = view;
            _shopModel = shopModel;
            _shopName = shopName;
            _buyCoefficient = buyCoefficient;
            _sellCoefficient = sellCoefficient;
        }

        public void Bind()
        {
            _view.SlotClicked += OnSlotClick;
            _shopModel.Changed += OnShopChanged;
        }

        public void Unbind()
        {
            if (_view != null)
            {
                _view.SlotClicked -= OnSlotClick;
            }

            if (_shopModel != null)
            {
                _shopModel.Changed -= OnShopChanged;
            }
        }

        public void Redraw()
        {
            var viewData = new List<ShopSlotViewData>();
            foreach (var (slot, snapshot) in _shopModel.Enumerate())
            {
                Sprite icon = null;
                string amount = "";
                string price = "";

                if (snapshot.HasValue)
                {
                    icon = snapshot.Value.Definition.Icon;

                    amount = _shopModel.IsInfinite(slot) ? "" : snapshot.Value.Amount.ToString();
                    price = _shopModel.TryGetBuyPrice(slot, _buyCoefficient, _sellCoefficient, out var buyPrice) ? buyPrice.ToString() : "";
                }
                var viewSlotData = new ShopSlotViewData(slot, snapshot.HasValue, icon, amount, price);
                viewData.Add(viewSlotData);
            }

            _view.Render(viewData, _shopName, _buyCoefficient, _sellCoefficient);
        }

        private void OnSlotClick(ShopSlot slot)
        {
            SlotClick?.Invoke(slot);
        }

        private void OnShopChanged()
        {
            Redraw();
        }
    }
}
