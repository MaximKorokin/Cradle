using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class EquipmentViewController : ViewControllerBase<EquipmentView>
    {
        private IEquipmentHudData _equipmentHudData;

        public void Bind(IEquipmentHudData equipmentHudData)
        {
            _equipmentHudData = equipmentHudData;
            _equipmentHudData.Changed += OnEquipmentChanged;
        }

        public void Unbind()
        {
            View.Unbind();

            if (_equipmentHudData != null)
            {
                _equipmentHudData.Changed -= OnEquipmentChanged;
                _equipmentHudData = null;
            }
        }

        protected override void OnRender()
        {
            View.RequestRender(_equipmentHudData);
        }

        private void OnEquipmentChanged()
        {
            Render();
        }
    }
}
