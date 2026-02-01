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

        public ItemStack Assemble(ItemStackSave save)
        {
            var definition = _itemCatalog.GetItemDefinition(save.Id);
            return new ItemStack(definition, save.InstanceData, save.Amount);
        }
    }
}
