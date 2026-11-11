using Assets._Game.Scripts.Shared.Unity;
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

        private readonly GameObjectCollection _instantiatedObjects = new();

        public event Action<string> ElementInfoClicked;
        public event Action<string> ElementActionClicked;

        private void Awake()
        {
            if (_itemTemplate != null)
                _itemTemplate.gameObject.SetActive(false);
        }

        public void Render(IEnumerable<SimpleListItemData> itemDefinitions, Func<SimpleListItemData, SimpleListItemView> templateProvider = null)
        {
            Clear();
            foreach (var itemDefinition in itemDefinitions)
            {
                var template = templateProvider != null ? templateProvider(itemDefinition) : _itemTemplate;
                var item = _instantiatedObjects.Instantiate(template, _itemsParent);
                item.Render(itemDefinition);
                item.InfoButtonClicked += OnItemInfoClicked;
                item.ActionButtonClicked += OnItemClicked;
                item.gameObject.SetActive(true);
            }
        }

        public void Clear()
        {
            _instantiatedObjects.Clear();
        }

        private void OnItemInfoClicked(string identifier)
        {
            ElementInfoClicked?.Invoke(identifier);
        }

        private void OnItemClicked(string identifier)
        {
            ElementActionClicked?.Invoke(identifier);
        }
    }
}
