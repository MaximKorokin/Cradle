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
                    model.AddToSlot(new(item.Type, item.Index), itemStack.Snapshot);
                }
            }

            return model;
        }

        public EquipmentModel Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModule<EquipmentModuleDefinition>(out var equipmentDefinitionModule))
                return null;

            var slots = equipmentDefinitionModule.EquipmentSlots.ToArray();
            return new EquipmentModel(slots, new DefaultEquipmentRules());
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
                        Stack = _itemStackAssembler.Save(slot.Snapshot.Value)
                    })
                    .ToArray()
            };
            return save;
        }
    }
}
