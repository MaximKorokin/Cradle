using Assets._Game.Scripts.Items.Equipment;
using System;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class EquipmentViewController
    {
        private readonly EquipmentView _equipmentView;

        public event Action<EquipmentSlotKey> SlotPointerDown
        {
            add => _equipmentView.SlotPointerDown += value;
            remove => _equipmentView.SlotPointerDown -= value;
        }
        public event Action<EquipmentSlotKey> SlotPointerUp
        {
            add => _equipmentView.SlotPointerUp += value;
            remove => _equipmentView.SlotPointerUp -= value;
        }

        public EquipmentViewController(EquipmentView equipmentView)
        {
            _equipmentView = equipmentView;
        }
    }
}
