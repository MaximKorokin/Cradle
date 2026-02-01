using Assets.CoreScripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Items
{
    public class ItemDefinitionCatalog
    {
        private readonly Dictionary<string, ItemDefinition> _items;

        public ItemDefinitionCatalog()
        {
            SLog.Log("Loading Item Catalog...");
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
    }
}
