using Assets._Game.Scripts.Items.Equipment;
using System;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class EquipmentViewController
    {
        private readonly EquipmentView _equipmentView;

        public event Action<EquipmentSlotKey> SlotClick
        {
            add => _equipmentView.SlotClick += value;
            remove => _equipmentView.SlotClick -= value;
        }

        public EquipmentViewController(EquipmentView equipmentView)
        {
            _equipmentView = equipmentView;
        }
    }
}
