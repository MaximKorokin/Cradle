using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Persistence;
using System.Linq;

namespace Assets._Game.Scripts.Items.Equipment
{
    public class EquipmentModelAssembler
    {
        private readonly ItemStackAssembler _itemStackAssembler;

        public EquipmentModelAssembler(ItemStackAssembler itemStackAssembler)
        {
            _itemStackAssembler = itemStackAssembler;
        }

        public EquipmentModel Apply(EquipmentModel model, EquipmentSave save)
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

        public EquipmentModel Create(EntityDefinition entityDefinition)
        {
            var slots = entityDefinition.EquipmentSlots.Select(slotType => new EquipmentSlot(slotType)).ToArray();
            return new EquipmentModel(slots);
        }
    }
}
