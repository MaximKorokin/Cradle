using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Shared.Extensions;
using System.Linq;

namespace Assets._Game.Scripts.Items.Inventory
{
    public class InventoryModelFactory
    {
        private readonly ItemStackFactory _itemStackFactory;

        public InventoryModelFactory(ItemStackFactory itemStackFactory)
        {
            _itemStackFactory = itemStackFactory;
        }

        public InventoryModel Apply(InventoryModel model, InventorySave save)
        {
            if (save != null && save.Items != null)
            {
                foreach (var item in save.Items)
                {
                    var itemStack = _itemStackFactory.CreateAndApply(item.Stack.ItemDefinitionId, item.Stack);
                    if (itemStack != null) model.AddToSlot(item.Slot, itemStack.Snapshot);
                }
            }

            return model;
        }

        public InventoryModel Create(int slotsAmount)
        {
            return new InventoryModel(slotsAmount, _itemStackFactory);
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
                        Stack = _itemStackFactory.Save(slot.Snapshot.Value)
                    })
                    .ToArray()
            };
            return save;
        }
    }
}
