using Assets._Game.Scripts.Items.Equipment;
using Assets.CoreScripts;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class EquipmentView : MonoBehaviour
    {
        [SerializeField]
        private EquipmentSlotView[] _slots;

        public event Action<EquipmentSlotKey> SlotPointerDown;
        public event Action<EquipmentSlotKey> SlotPointerUp;

        public void Bind()
        {
            foreach (var slot in _slots)
            {
                slot.PointerDown -= OnSlotPointerDown;
                slot.PointerDown += OnSlotPointerDown;
                slot.PointerUp -= OnSlotPointerUp;
                slot.PointerUp += OnSlotPointerUp;
            }
        }

        public void Render(EquipmentModel equipmentModel)
        {
            var slotTypeCounts = new Dictionary<EquipmentSlotType, int>();
            foreach (var slot in _slots)
            {
                var count = 0;
                if (slotTypeCounts.TryGetValue(slot.SlotType, out var slotTypeCount))
                    count = slotTypeCount;
                var slotKey = new EquipmentSlotKey(slot.SlotType, count);
                var itemStack = equipmentModel.Get(slotKey);
                slot.Render(itemStack);
                slot.Bind(slotKey);
                slotTypeCounts[slot.SlotType] = count + 1;
            }
        }

        private void OnSlotPointerDown(EquipmentSlotKey slot)
        {
            SlotPointerDown?.Invoke(slot);
        }

        private void OnSlotPointerUp(EquipmentSlotKey slot)
        {
            SlotPointerUp?.Invoke(slot);
        }
    }
}
