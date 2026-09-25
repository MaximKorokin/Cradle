using Assets._Game.Scripts.Infrastructure.Storage;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Items
{
    public sealed class ItemSetDefinitionCatalog : DefinitionCatalogBase<ItemSetDefinition>
    {
        private Dictionary<string, ItemSetDefinition> _itemToSetsCache;

        protected override void OnDefinitionsLoaded(IEnumerable<ItemSetDefinition> definitions)
        {
            base.OnDefinitionsLoaded(definitions);

            BuildItemToSetsCache(definitions);
        }

        private void BuildItemToSetsCache(IEnumerable<ItemSetDefinition> definitions)
        {
            _itemToSetsCache = new();

            foreach (var set in definitions)
            {
                foreach (var item in set.SetItems)
                {
                    if (item == null) continue;

                    if (!_itemToSetsCache.ContainsKey(item.Id))
                    {
                        _itemToSetsCache[item.Id] = set;
                    }
                    else
                    {
                        SLog.Warn($"Item '{item.Name}' (ID: {item.Id}) is already a part of set {_itemToSetsCache[item.Id].Id} but also found {set.Id}. Only the first set will be used.");
                    }
                }
            }
        }

        public ItemSetDefinition GetSetByItem(string itemId)
        {
            if (_itemToSetsCache.TryGetValue(itemId, out var sets))
            {
                return sets;
            }

            return null;
        }

        public ItemSetDefinition GetSetByItem(ItemDefinition itemDefinition)
        {
            return itemDefinition != null ? GetSetByItem(itemDefinition.Id) : null;
        }
    }
}
