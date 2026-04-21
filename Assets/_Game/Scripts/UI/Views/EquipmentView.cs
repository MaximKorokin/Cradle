using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.UI.DataAggregators;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class EquipmentView : MonoBehaviour
    {
        [SerializeField]
        private EquipmentSlotView[] _slots;

        private IEquipmentHudData _equipmentHudData;

        public event Action<EquipmentSlotKey> SlotClick;

        public void Bind()
        {
            foreach (var slot in _slots)
            {
                slot.PointerClick += OnSlotPointerClick;
            }
        }

        public void Render(IEquipmentHudData equipmentHudData)
        {
            _equipmentHudData = equipmentHudData;
            _equipmentHudData.Changed += OnEquipmentHudDataChanged;
            OnEquipmentHudDataChanged();
        }

        private void OnEquipmentHudDataChanged()
        {
            var slotTypeCounts = new Dictionary<EquipmentSlotType, int>();
            foreach (var slot in _slots)
            {
                var count = 0;
                if (slotTypeCounts.TryGetValue(slot.SlotType, out var slotTypeCount))
                    count = slotTypeCount;
                var slotKey = new EquipmentSlotKey(slot.SlotType, count);
                var itemStack = _equipmentHudData.EquipmentModel.Get(slotKey);
                slot.Render(itemStack);
                slot.Bind(slotKey);
                slotTypeCounts[slot.SlotType] = count + 1;
            }
        }

        public void Unbind()
        {
            foreach (var slot in _slots)
            {
                slot.PointerClick -= OnSlotPointerClick;
            }

            if (_equipmentHudData != null)
            {
                _equipmentHudData.Changed -= OnEquipmentHudDataChanged;
                _equipmentHudData = null;
            }
        }

        private void OnSlotPointerClick(EquipmentSlotKey slot)
        {
            SlotClick?.Invoke(slot);
        }
    }
}
