using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Common
{
    public sealed class TabbedListViewCollection
    {
        private readonly SelectableTabsController _tabsController;
        private readonly SimpleListView _listViewTemplate;
        private readonly List<ListViewRegistration> _listViews = new();

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
            var listView = UnityEngine.Object.Instantiate(_listViewTemplate);
            listView.Render(items, templateProvider);

            if (infoClicked != null)
                listView.ElementInfoClicked += infoClicked;
            if (actionClicked != null)
                listView.ElementActionClicked += actionClicked;

            _tabsController.AddTab(new TabData(id, title, listView.transform as RectTransform));
            _listViews.Add(new ListViewRegistration(listView, infoClicked, actionClicked));
        }

        public void Clear()
        {
            foreach (var registration in _listViews)
            {
                if (registration.InfoClicked != null)
                    registration.ListView.ElementInfoClicked -= registration.InfoClicked;
                if (registration.ActionClicked != null)
                    registration.ListView.ElementActionClicked -= registration.ActionClicked;

                registration.ListView.Clear();
                UnityEngine.Object.Destroy(registration.ListView.gameObject);
            }

            _listViews.Clear();
            _tabsController.ClearTabs();
        }

        private sealed class ListViewRegistration
        {
            public SimpleListView ListView { get; }
            public Action<string> InfoClicked { get; }
            public Action<string> ActionClicked { get; }

            public ListViewRegistration(SimpleListView listView, Action<string> infoClicked, Action<string> actionClicked)
            {
                ListView = listView;
                InfoClicked = infoClicked;
                ActionClicked = actionClicked;
            }
        }
    }
}
