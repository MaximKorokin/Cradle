using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.UI.DataAggregators;
using System;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class EquipmentViewController : ViewControllerBase<EquipmentView>
    {
        private IEquipmentHudData _equipmentHudData;

        public void Bind(IEquipmentHudData equipmentHudData)
        {
            _equipmentHudData = equipmentHudData;
            _equipmentHudData.Changed += Redraw;
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
