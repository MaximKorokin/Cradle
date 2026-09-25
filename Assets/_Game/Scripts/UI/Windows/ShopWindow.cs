using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class ShopWindow : UIWindowBase
    {
        [SerializeField]
        private ShopView _shopView;

        public ShopView ShopView => _shopView;
    }
}
