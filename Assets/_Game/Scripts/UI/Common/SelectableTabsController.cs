using Assets.CoreScripts;
using System;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Common
{
    public class SelectableTabsController : MonoBehaviour
    {
        [SerializeField]
        private SelectableTab[] _initialTabs;
        [Space]
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
            if (index >= 0 && index < _tabsGroup.SelectableElements.Count)
            {
                _tabsGroup.Select(_tabsGroup.SelectableElements.ElementAt(index));
            }
        }

        public int GetSelectedTabIndex()
        {
            var tabs = _tabsGroup.SelectableElements.ToArray();
            for (int i = 0; i < tabs.Length; i++)
            {
                if (tabs[i].IsSelected)
                    return i;
            }
            return 0;
        }

        public void AddTab(TabData tabData)
        {
            tabData.Content.transform.SetParent(_tabContentsContainer, false);

            var newTab = Instantiate(_tabTemplate, _tabsContainer);
            newTab.Initialize(tabData.Id, tabData.Title, tabData.Content);
            newTab.gameObject.SetActive(true);

            _tabsGroup.AddSelectable(newTab);
        }

        public void RemoveTab(string id)
        {
            var tabToRemove = _tabsGroup.SelectableElements.FirstOrDefault(x => x.Id == id);
            if (tabToRemove != null)
            {
                _tabsGroup.RemoveSelectable(tabToRemove);
                Destroy(tabToRemove.gameObject);
            }
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
        public readonly string Id;
        public readonly string Title;
        public readonly RectTransform Content;

        public TabData(string title, RectTransform content) : this(title, title, content) { }
        public TabData(string id, string title, RectTransform content)
        {
            Id = id;
            Title = title;
            Content = content;
        }
    }
}
