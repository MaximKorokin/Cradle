using Assets._Game.Scripts.Infrastructure.Persistence;

namespace Assets._Game.Scripts.Items
{
    public class ItemStackFactory
    {
        private readonly ItemDefinitionCatalog _itemCatalog;
        private readonly ItemInstanceDataFactory _instanceDataFactory;

        public ItemStackFactory(ItemDefinitionCatalog itemCatalog, ItemInstanceDataFactory instanceDataFactory)
        {
            _itemCatalog = itemCatalog;
            _instanceDataFactory = instanceDataFactory;
        }

        public ItemStack Create(string definitionId, int amount)
        {
            return Create(definitionId, _instanceDataFactory.Create(definitionId), amount);
        }

        public ItemStack Create(string definitionId, IItemInstanceData itemInstanceData, int amount)
        {
            var definition = _itemCatalog.Get(definitionId);
            return new ItemStack(definition, itemInstanceData, amount);
        }

        public ItemStack Apply(ItemStack itemStack, ItemStackSave save)
        {
            var definition = _itemCatalog.Get(save.ItemDefinitionId);
            itemStack.Definition = definition;
            itemStack.Amount = save.Amount;
            itemStack.InstanceData = _instanceDataFactory.Apply(save.InstanceData);
            return itemStack;
        }

        public ItemStackSave Save(ItemStackSnapshot itemStack)
        {
            return new ItemStackSave
            {
                ItemDefinitionId = itemStack.Definition.Id,
                Amount = itemStack.Amount,
                InstanceData = _instanceDataFactory.Save(itemStack.InstanceData)
            };
        }
    }
}
