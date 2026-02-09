using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Common
{
    public class SimpleListView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _itemsParent;
        [SerializeField]
        private SimpleListItemView _itemTemplate;

        private readonly List<SimpleListItemView> _items = new();

        public event Action<object> ItemClicked;

        private void Awake()
        {
            _itemTemplate.gameObject.SetActive(false);
        }

        public void Render(IEnumerable<SimpleListItemDefinition> itemDefinitions)
        {
            Clear();
            foreach (var itemDefinition in itemDefinitions)
            {
                var item = Instantiate(_itemTemplate);
                _items.Add(item);
                item.transform.SetParent(_itemsParent, false);
                item.Render(itemDefinition);
                item.Button1Clicked += OnItemClicked;
                item.gameObject.SetActive(true);
            }
        }

        public void Clear()
        {
            foreach (var item in _items)
            {
                item.Button1Clicked -= OnItemClicked;
                item.Clear();
                Destroy(item);
            }

            _items.Clear();
        }

        private void OnItemClicked(object obj)
        {
            ItemClicked?.Invoke(obj);
        }
    }
}
