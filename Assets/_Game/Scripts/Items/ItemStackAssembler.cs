using Assets._Game.Scripts.Infrastructure.Persistence;

namespace Assets._Game.Scripts.Items
{
    public class ItemStackAssembler
    {
        private readonly ItemDefinitionCatalog _itemCatalog;

        public ItemStackAssembler(ItemDefinitionCatalog itemCatalog)
        {
            _itemCatalog = itemCatalog;
        }

        public ItemStack Apply(ItemStack itemStack, ItemStackSave save)
        {
            var definition = _itemCatalog.GetItemDefinition(save.ItemDefinitionId);
            itemStack.Definition = definition;
            itemStack.Amount = save.Amount;
            itemStack.Instance = save.InstanceData;
            return itemStack;
        }

        public ItemStack Create(string definitionId, int amount)
        {
            var definition = _itemCatalog.GetItemDefinition(definitionId);
            return new ItemStack(definition, new EmptyInstanceData(), amount);
        }
    }
}
