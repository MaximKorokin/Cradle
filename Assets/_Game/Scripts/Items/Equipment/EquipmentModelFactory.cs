using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Shared.Extensions;
using System.Linq;

namespace Assets._Game.Scripts.Items.Equipment
{
    public class EquipmentModelFactory
    {
        private readonly ItemStackFactory _itemStackFactory;

        public EquipmentModelFactory(ItemStackFactory itemStackFactory)
        {
            _itemStackFactory = itemStackFactory;
        }

        public EquipmentModel Apply(EquipmentModel model, EquipmentSave save)
        {
            if (save != null && save.Items != null)
            {
                foreach (var item in save.Items)
                {
                    var itemStack = _itemStackFactory.CreateAndApply(item.Stack.ItemDefinitionId, item.Stack);
                    model.AddToSlot(new(item.Type, item.Index), itemStack.Snapshot);
                }
            }

            return model;
        }

        public EquipmentModel Create(EquipmentSlotType[] slots)
        {
            return new EquipmentModel(slots, new DefaultEquipmentRules(), _itemStackFactory);
        }

        public EquipmentSave Save(EquipmentModel model)
        {
            var save = new EquipmentSave
            {
                Items = model.Enumerate()
                    .Where(slot => slot.Snapshot != null)
                    .Select(slot => new EquipmentSlotSave
                    {
                        Type = slot.Slot.SlotType,
                        Index = slot.Slot.Index,
                        Stack = _itemStackFactory.Save(slot.Snapshot.Value)
                    })
                    .ToArray()
            };
            return save;
        }
    }
}
