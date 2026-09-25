using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.UI.DataAggregators;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class EquipmentView : UIViewBase<IEquipmentHudData>
    {
        [SerializeField]
        private EquipmentSlotView[] _slots;

        private IEquipmentHudData _equipmentHudData;

        public override void Render(IEquipmentHudData equipmentHudData)
        {
            _equipmentHudData = equipmentHudData;

            // Find all equipped items that have secondary slots and create a mapping of those secondary slots to the item occupying them
            var blockedSlots = _equipmentHudData.EquipmentModel.Enumerate()
                .Where(x => x.Snapshot != null
                    && x.Snapshot.Value.Definition.TryGetTrait<EquippableTrait>(out var equippableTrait)
                    && equippableTrait.SecondarySlots.Length > 0)
                .SelectMany(x => x.Snapshot.Value.Definition.GetTrait<EquippableTrait>().SecondarySlots
                    .Select(y => (Slot: y, Item: x.Snapshot.Value)))
                .ToDictionary(x => x.Slot, x => x.Item);

            var slotTypeCounts = new Dictionary<EquipmentSlotType, int>();
            foreach (var slot in _slots)
            {
                var count = 0;
                if (slotTypeCounts.TryGetValue(slot.SlotType, out var slotTypeCount))
                    count = slotTypeCount;

                var slotKey = new EquipmentSlotKey(slot.SlotType, count);
                var itemStack = _equipmentHudData.EquipmentModel.Get(slotKey);

                // Show item in slot or item that is blocking the slot if there is one, but indicate that it is blocked
                if (itemStack == null && blockedSlots.TryGetValue(slot.SlotType, out var blockedItemStack))
                    slot.Render(blockedItemStack, true);
                else
                    slot.Render(itemStack, false);

                slot.SlotView.Bind(equipmentHudData.ContainerPath, slotKey.ToInt64());
                slotTypeCounts[slot.SlotType] = count + 1;
            }
        }

        public void Unbind()
        {
            _equipmentHudData = null;
        }
    }
}
