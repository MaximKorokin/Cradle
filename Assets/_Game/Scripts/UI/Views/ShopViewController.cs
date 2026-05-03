using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.UI.Views;
using System;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ShopViewController
    {
        private ShopView _view;
        private ShopModel _shopModel;
        private int _playerGold;

        public event Action<ShopSlot> SlotClick;

        public void Initialize(ShopView view, ShopModel shopModel)
        {
            _view = view;
            _shopModel = shopModel;
        }

        public void Bind()
        {
            _view.SlotClick += OnSlotClick;
            _shopModel.Changed += OnShopChanged;
        }

        public void Unbind()
        {
            if (_view != null)
            {
                _view.SlotClick -= OnSlotClick;
                _view.Unbind();
            }

            if (_shopModel != null)
            {
                _shopModel.Changed -= OnShopChanged;
            }
        }

        public void Redraw()
        {
            _view.Render(_shopModel, "Shop", _playerGold);
        }

        public void SetPlayerGold(int gold)
        {
            _playerGold = gold;
            Redraw();
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
