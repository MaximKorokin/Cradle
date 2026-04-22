using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.UI.DataAggregators;
using System;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class EquipmentViewController : ViewControllerBase<EquipmentView>
    {
        private IEquipmentHudData _equipmentHudData;

        public event Action<EquipmentSlotKey> SlotClick
        {
            add => View.SlotClick += value;
            remove => View.SlotClick -= value;
        }

        public void Bind(IEquipmentHudData equipmentHudData)
        {
            _equipmentHudData = equipmentHudData;
            _equipmentHudData.Changed += Redraw;

            View.Bind();
        }

        public void Unbind()
        {
            View.Unbind();

            if (_equipmentHudData != null)
                _equipmentHudData.Changed -= Redraw;
            _equipmentHudData = null;
        }

        public void Redraw()
        {
            View.Render(_equipmentHudData);
        }
    }
}
