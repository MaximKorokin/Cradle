using Assets._Game.Scripts.UI.Common;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class ShopWindow : UIWindowBase
    {
        [SerializeField]
        private SelectableTabsController _shopTabsController;
        [SerializeField]
        private SimpleListView _shopTabContentTemplate;
    }
}
