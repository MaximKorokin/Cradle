using Assets._Game.Scripts.UI.Common;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class CraftingWindow : UIWindowBase
    {
        [SerializeField]
        private SelectableTabsController _craftingTabsController;
        [SerializeField]
        private SimpleListView _craftingTabContentTemplate;
    }
}
