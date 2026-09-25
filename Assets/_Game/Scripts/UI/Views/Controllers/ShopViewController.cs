using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;
using Assets._Game.Scripts.UI.Views.Widgets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ShopViewController : ViewControllerBase<ShopView>
    {
        private ShopModel _shopModel;
        private string _shopName;
        private float _buyCoefficient;
        private float _sellCoefficient;

        public void InitializeShop(ShopView view, ShopModel shopModel, string shopName, float buyCoefficient, float sellCoefficient)
        {
            Initialize(view);
            _shopModel = shopModel;
            _shopName = shopName;
            _buyCoefficient = buyCoefficient;
            _sellCoefficient = sellCoefficient;
        }

        public void Bind()
        {
            _shopModel.Changed += OnShopChanged;
        }

        public void Unbind()
        {
            if (_shopModel != null)
            {
                _shopModel.Changed -= OnShopChanged;
            }
        }

        protected override void OnRender()
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

            View.RequestRender((viewData, _shopName, _buyCoefficient, _sellCoefficient));
        }

        private void OnShopChanged()
        {
            Render();
        }

        public override void Dispose()
        {
            Unbind();
            base.Dispose();
        }
    }
}
