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

        public event Action<EquipmentSlotType> SlotPointerDown;
        public event Action<EquipmentSlotType> SlotPointerUp;

        public void Bind()
        {
            foreach (var slot in _slots)
            {
                slot.Bind(slot.SlotType);
                slot.PointerDown -= OnSlotPointerDown;
                slot.PointerDown += OnSlotPointerDown;
                slot.PointerUp -= OnSlotPointerUp;
                slot.PointerUp += OnSlotPointerUp;
            }
        }

        public void Render(EquipmentModel equipmentModel)
        {
            // Hide all slots initially
            for (int i = 0; i < _slots.Length; i++)
                _slots[i].gameObject.SetActive(false);

            // Create pools of available slots by type
            var pools = new Dictionary<EquipmentSlotType, Queue<EquipmentSlotView>>();
            for (int i = 0; i < _slots.Length; i++)
            {
                var v = _slots[i];
                if (!pools.TryGetValue(v.SlotType, out var q))
                    pools[v.SlotType] = q = new Queue<EquipmentSlotView>();

                q.Enqueue(v);
            }

            // Render each equipped item into an available slot
            foreach (var (slotType, stack) in equipmentModel.Enumerate())
            {
                if (!pools.TryGetValue(slotType, out var q) || q.Count == 0)
                {
                    SLog.Warn($"No available slot for equipment type {slotType}");
                    continue;
                }

                var view = q.Dequeue();
                view.Render(stack);
                view.gameObject.SetActive(true);
            }
        }

        private void OnSlotPointerDown(EquipmentSlotType slotType)
        {
            SlotPointerDown?.Invoke(slotType);
        }

        private void OnSlotPointerUp(EquipmentSlotType slotType)
        {
            SlotPointerUp?.Invoke(slotType);
        }
    }
}
