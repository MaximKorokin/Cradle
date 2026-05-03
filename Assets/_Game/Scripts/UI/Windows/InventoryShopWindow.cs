using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class InventoryShopWindow : UIWindowBase
    {
        [SerializeField]
        private InventoryView _inventoryView;
        [SerializeField]
        private ShopView _shopView;

        public InventoryView InventoryView => _inventoryView;
        public ShopView ShopView => _shopView;
    }
}
