using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class ShopWindow : UIWindowBase
    {
        [field: SerializeField]
        public ShopView ShopView { get; private set; }
    }
}
