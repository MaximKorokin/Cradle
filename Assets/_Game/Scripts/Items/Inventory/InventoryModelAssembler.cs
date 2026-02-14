using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Shared.Extensions;
using System.Linq;

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
                foreach (var item in save.Items)
                {
                    var itemStack = _itemStackAssembler.CreateAndApply(item.Stack.ItemDefinitionId, item.Stack);
                    model.AddToSlot(item.Slot, itemStack.Snapshot);
                }
            }

            return model;
        }

        public InventoryModel Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModule<InventoryModuleDefinition>(out var inventoryDefinitionModule)) 
                return null;

            return new InventoryModel(inventoryDefinitionModule.SlotsAmount);
        }

        public InventorySave Save(InventoryModel model)
        {
            var save = new InventorySave
            {
                Items = model.Enumerate()
                    .Where(slot => slot.Snapshot != null)
                    .Select(slot => new InventorySlotSave
                    {
                        Slot = slot.Slot,
                        Stack = _itemStackAssembler.Save(slot.Snapshot.Value)
                    })
                    .ToArray()
            };
            return save;
        }
    }
}
