using Assets._Game.Scripts.Shared.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Common
{
    public sealed class TabbedListViewCollection
    {
        private readonly SelectableTabsController _tabsController;
        private readonly SimpleListView _listViewTemplate;

        private readonly GameObjectCollection _listViews = new();

        public TabbedListViewCollection(SelectableTabsController tabsController, SimpleListView template)
        {
            _tabsController = tabsController;
            _listViewTemplate = template;
        }

        public void AddTab(
            string id,
            string title,
            IEnumerable<SimpleListItemData> items,
            Action<string> infoClicked = null,
            Action<string> actionClicked = null,
            Func<SimpleListItemData, SimpleListItemView> templateProvider = null)
        {
            var listView = _listViews.Instantiate(_listViewTemplate);
            listView.Render(items, templateProvider);

            if (infoClicked != null)
                listView.ElementInfoClicked += infoClicked;
            if (actionClicked != null)
                listView.ElementActionClicked += actionClicked;

            _tabsController.AddTab(new TabData(id, title, listView.transform as RectTransform));
        }

        public void Clear()
        {
            _listViews.Clear();
            _tabsController.ClearTabs();
        }
    }
}
