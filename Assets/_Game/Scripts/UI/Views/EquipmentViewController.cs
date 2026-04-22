using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.UI.DataAggregators;
using System;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class EquipmentViewController
    {
        private readonly EquipmentView _equipmentView;

        private IEquipmentHudData _equipmentHudData;

        public event Action<EquipmentSlotKey> SlotClick
        {
            add => _equipmentView.SlotClick += value;
            remove => _equipmentView.SlotClick -= value;
        }

        public EquipmentViewController(EquipmentView equipmentView)
        {
            _equipmentView = equipmentView;
        }

        public void Bind(IEquipmentHudData equipmentHudData)
        {
            _equipmentHudData = equipmentHudData;
            _equipmentHudData.Changed += Redraw;

            _equipmentView.Bind();
        }

        public void Unbind()
        {
            _equipmentView.Unbind();

            if (_equipmentHudData != null)
                _equipmentHudData.Changed -= Redraw;
            _equipmentHudData = null;
        }

        public void Redraw()
        {
            _equipmentView.Render(_equipmentHudData);
        }
    }
}
