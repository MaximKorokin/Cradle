using Assets.CoreScripts;
using System;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Common
{
    public class SelectableTabsController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _tabsContainer;
        [SerializeField]
        private SelectableTab _tabTemplate;
        [Space]
        [SerializeField]
        private RectTransform _tabContentsContainer;

        private readonly SelectableElementsGroup<SelectableTab> _tabsGroup = new();

        private void Awake()
        {
            _tabTemplate.gameObject.SetActive(false);
        }

        public void SelectTab(int index)
        {
            _tabsGroup.Select(_tabsGroup.SelectableElements.ElementAt(index));
        }

        public void AddTab(TabData tabData)
        {
            tabData.Content.transform.SetParent(_tabContentsContainer, false);

            var newTab = Instantiate(_tabTemplate, _tabsContainer);
            newTab.Initialize(tabData.Title, tabData.Content);
            newTab.gameObject.SetActive(true);

            _tabsGroup.AddSelectable(newTab);
        }

        public void ClearTabs()
        {
            var tabsToRemove = _tabsGroup.SelectableElements.ToArray();
            tabsToRemove.ForEach(x =>
            {
                _tabsGroup.RemoveSelectable(x);
                Destroy(x.gameObject);
            });
        }
    }

    [Serializable]
    public readonly struct TabData
    {
        public readonly string Title;
        public readonly RectTransform Content;

        public TabData(string title, RectTransform content)
        {
            Title = title;
            Content = content;
        }
    }
}
