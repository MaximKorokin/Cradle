using Assets._Game.Scripts.UI.Views.Widgets;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.UI.DataAggregators;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class EquipmentView : UIViewBase<IEquipmentViewData>
    {
        [SerializeField]
        private EquipmentSlotWidget[] _slots;

        protected override void Render(IEquipmentViewData equipmentViewData)
        {
            var blockedSlots = equipmentViewData.EquipmentModel
                .EnumerateBlockedSlots()
                .GroupBy(x => x.BlockedSlot.SlotType)
                .ToDictionary(x => x.Key, x => x.First());

            var slotTypeCounts = new Dictionary<EquipmentSlotType, int>();
            foreach (var slot in _slots)
            {
                var count = 0;
                if (slotTypeCounts.TryGetValue(slot.SlotType, out var slotTypeCount))
                    count = slotTypeCount;

                var slotKey = new EquipmentSlotKey(slot.SlotType, count);
                var itemStack = equipmentViewData.EquipmentModel.Get(slotKey);

                // Show item in slot or item that is blocking the slot if there is one, but indicate that it is blocked
                long? blockingSlotIndex = null;
                if (itemStack == null && blockedSlots.TryGetValue(slot.SlotType, out var blocker))
                {
                    slot.Render(blocker.Item, true);
                    blockingSlotIndex = blocker.BlockingSlot.ToInt64();
                }
                else
                {
                    slot.Render(itemStack, false);
                }

                slot.Bind(equipmentViewData.ContainerPath, slotKey.ToInt64(), blockingSlotIndex);
                slotTypeCounts[slot.SlotType] = count + 1;
            }
        }
    }
}
