using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class CheatsWindow : UIWindow
    {
        [SerializeField]
        private SimpleListView _itemsListView;

        private Dictionary<object, ItemDefinition> _itemDefinitions;

        public event Action<ItemDefinition> ItemDefinitionClicked;

        public void Render(IEnumerable<ItemDefinition> itemDefinitions)
        {
            _itemDefinitions = itemDefinitions.ToDictionary(d => (object)d.Id, d => d);
            _itemsListView.Render(itemDefinitions.Select(d => new SimpleListItemDefinition()
            {
                Identifier = d.Id,
                Sprite = d.Icon,
                Text = d.Name
            }));
            _itemsListView.ItemClicked += OnItemDefinitionClicked;
        }

        public void Clear()
        {
            _itemsListView.ItemClicked -= OnItemDefinitionClicked;
            _itemsListView.Clear();
        }

        private void OnItemDefinitionClicked(object obj)
        {
            if (_itemDefinitions.TryGetValue(obj, out var itemDefinition))
            {
                ItemDefinitionClicked?.Invoke(itemDefinition);
            }
        }
    }
}
