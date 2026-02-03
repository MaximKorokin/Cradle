using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Shared.Extensions;
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
                foreach (var item in save.Items)
                {
                    var itemStack = _itemStackAssembler.CreateAndApply(item.Stack.ItemDefinitionId, item.Stack);
                    model.Put(item.Slot, itemStack);
                }
            }

            return model;
        }

        public EquipmentModel Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModule<EquipmentDefinitionModule>(out var equipmentDefinitionModule))
                return null;

            var slots = equipmentDefinitionModule.EquipmentSlots.Select(slotType => new EquipmentSlot(slotType)).ToArray();
            return new EquipmentModel(slots);
        }

        public EquipmentSave Save(EquipmentModel model)
        {
            var save = new EquipmentSave();
            save.Items = model.Enumerate()
                .Where(slot => slot.Stack != null)
                .Select(slot => new EquipmentSlotSave
                {
                    Slot = slot.Index,
                    Stack = _itemStackAssembler.Save(slot.Stack)
                })
                .ToArray();
            return save;
        }
    }
}
