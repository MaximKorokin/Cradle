using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Infrastructure.Persistence.Codecs;
using Assets._Game.Scripts.Items.Traits;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Items
{
    public sealed class ItemInstanceDataFactory
    {
        private readonly ItemDefinitionCatalog _itemCatalog;
        private readonly CodecRegistry _codecRegistry;

        public ItemInstanceDataFactory(ItemDefinitionCatalog itemCatalog, CodecRegistry codecRegistry)
        {
            _itemCatalog = itemCatalog;
            _codecRegistry = codecRegistry;
        }

        public IItemInstanceData Create(string definitionId)
        {
            var definition = _itemCatalog.Get(definitionId);
            if (definition == null)
            {
                SLog.Error($"Item definition with ID '{definitionId}' not found.");
                return null;
            }

            // Create and fill instance data based on the traits of the item definition
            var instanceDataList = GetDefaultInstanceDataList(definition);

            // Return instance data
            if (instanceDataList.Count > 0)
            {
                return new CompositeInstanceData(instanceDataList);
            }
            else
            {
                return new EmptyInstanceData();
            }
        }

        public IItemInstanceData Apply(EncodedSaveData[] save, ItemDefinition itemDefinition)
        {
            var defaultInstanceData = Create(itemDefinition.Id);

            if (defaultInstanceData == null)
            {
                return null;
            }

            if (save == null || save.Length == 0)
            {
                return defaultInstanceData;
            }

            if (defaultInstanceData is not CompositeInstanceData compositeDefaultInstanceData)
            {
                return defaultInstanceData;
            }

            // Decode the saved data into instance data objects
            var instanceDataList = new List<IItemInstanceData>();
            foreach (var item in save)
            {
                if (_codecRegistry.DecodeOrNull(item, itemDefinition) is IItemInstanceData decodedData)
                {
                    instanceDataList.Add(decodedData);
                }
                else
                {
                    SLog.Warn($"Failed to decode instance data of type '{item.Type}' for item definition '{itemDefinition.Id}'.");
                }
            }

            // Add any default instance data that was not present in the saved data
            var nonSavedInstanceData = compositeDefaultInstanceData.Children
                .Where(defaultChild => !instanceDataList.Any(savedChild => savedChild.GetType() == defaultChild.GetType()))
                .ToList();

            instanceDataList.AddRange(nonSavedInstanceData);

            return new CompositeInstanceData(instanceDataList);
        }

        public EncodedSaveData[] Save(IItemInstanceData instanceData)
        {
            if (instanceData is CompositeInstanceData composite)
            {
                return composite.Children
                    .Select(child => _codecRegistry.EncodeOrNull(child))
                    .Where(encoded => encoded != null)
                    .ToArray();
            }
            else
            {
                var encoded = _codecRegistry.EncodeOrNull(instanceData);
                return encoded != null ? new[] { encoded } : new EncodedSaveData[0];
            }
        }

        private List<IItemInstanceData> GetDefaultInstanceDataList(ItemDefinition itemDefinition)
        {
            var instanceDataList = new List<IItemInstanceData>();

            if (itemDefinition.TryGetTrait<UsableTrait>(out var usableTrait))
            {
                instanceDataList.Add(new CooldownInstanceData(usableTrait.Cooldown));
            }

            if (itemDefinition.TryGetTrait<EnchantableTrait>(out var enchantableTrait))
            {
                instanceDataList.Add(new EnchantInstanceData(0));
            }

            return instanceDataList;
        }
    }
}
