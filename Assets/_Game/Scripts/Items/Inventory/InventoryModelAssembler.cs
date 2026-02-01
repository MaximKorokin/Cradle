using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Persistence;

namespace Assets._Game.Scripts.Items.Inventory
{
    public class InventoryModelAssembler
    {
        private readonly ItemStackAssembler _itemStackAssembler;

        public InventoryModelAssembler(ItemStackAssembler itemStackAssembler)
        {
            _itemStackAssembler = itemStackAssembler;
        }

        public InventoryModel Apply(InventoryModel model, InventorySave save)
        {
            if (save != null && save.Items != null)
            {
                foreach (var (index, itemStackSave) in save.Items)
                {
                    var itemStack = _itemStackAssembler.Assemble(itemStackSave);
                    model.Put(index, itemStack);
                }
            }

            return model;
        }

        public InventoryModel Create(EntityDefinition entityDefinition)
        {
            return new InventoryModel(entityDefinition.InventorySlotsAmount);
        }
    }
}
