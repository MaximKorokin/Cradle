using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Infrastructure.Persistence.Codecs;
using Assets._Game.Scripts.Items.Traits;

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

            if (definition.TryGetTrait<UsableTrait>(out var usableTrait))
            {
                return new CooldownInstanceData(usableTrait.Cooldown);
            }

            return new EmptyInstanceData();
        }

        public IItemInstanceData Apply(EncodedSaveData save, ItemDefinition itemDefinition)
        {
            return _codecRegistry.DecodeOrNull(save, itemDefinition) as IItemInstanceData;
        }

        public EncodedSaveData Save(IItemInstanceData instanceData)
        {
            return _codecRegistry.EncodeOrNull(instanceData);
        }
    }
}
