using Assets.CoreScripts;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Common
{
    public class SelectableTabsController : MonoBehaviourBase
    {
        [SerializeField]
        private SelectableTab[] _tabs;
        [SerializeField]
        private int _defaultTabindex;

        private readonly SelectableElementsGroup<SelectableTab> _tabsGroup = new();

        protected override void Start()
        {
            base.Start();
            _tabs.ForEach(x => _tabsGroup.AddSelectable(x));
            _tabs[_defaultTabindex].SetSelection(true, false);
        }
    }
}
