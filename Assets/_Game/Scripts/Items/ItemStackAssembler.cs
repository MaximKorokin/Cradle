using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Infrastructure.Persistence.Codecs;

namespace Assets._Game.Scripts.Items
{
    public class ItemStackAssembler
    {
        private readonly ItemDefinitionCatalog _itemCatalog;
        private readonly CodecRegistry _codecRegistry;

        public ItemStackAssembler(ItemDefinitionCatalog itemCatalog, CodecRegistry codecRegistry)
        {
            _itemCatalog = itemCatalog;
            _codecRegistry = codecRegistry;
        }

        public ItemStack Apply(ItemStack itemStack, ItemStackSave save)
        {
            var definition = _itemCatalog.Get(save.ItemDefinitionId);
            itemStack.Definition = definition;
            itemStack.Amount = save.Amount;
            itemStack.InstanceData = _codecRegistry.DecodeOrNull(save.InstanceData) as IItemInstanceData;
            return itemStack;
        }

        public ItemStack Create(string definitionId, int amount)
        {
            var definition = _itemCatalog.Get(definitionId);
            return new ItemStack(definition, new EmptyInstanceData(), amount);
        }

        public ItemStackSave Save(ItemStackSnapshot itemStack)
        {
            return new ItemStackSave
            {
                ItemDefinitionId = itemStack.Definition.Id,
                Amount = itemStack.Amount,
                InstanceData = _codecRegistry.EncodeOrNull(itemStack.InstanceData)
            };
        }
    }
}
