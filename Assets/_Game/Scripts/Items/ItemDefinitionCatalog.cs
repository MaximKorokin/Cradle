using Assets.CoreScripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Items
{
    public class ItemDefinitionCatalog : IEnumerable<ItemDefinition>
    {
        private readonly Dictionary<string, ItemDefinition> _items;

        public ItemDefinitionCatalog()
        {
            _items = Resources.LoadAll<ItemDefinition>("").ToDictionary(x => x.Id, x => x);
        }

        public ItemDefinition GetItemDefinition(string id)
        {
            if (_items.TryGetValue(id, out var itemDefinition))
            {
                return itemDefinition;
            }

            SLog.Error($"Item with id '{id}' not found in catalog.");
            return null;
        }

        public IEnumerator<ItemDefinition> GetEnumerator()
        {
            return _items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
