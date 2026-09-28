using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class EquipmentViewController : ViewControllerBase<EquipmentView>
    {
        private IEquipmentViewData _equipmentViewData;

        public void Bind(IEquipmentViewData equipmentViewData)
        {
            _equipmentViewData = equipmentViewData;
            _equipmentViewData.Changed += OnEquipmentChanged;
        }

        public void Unbind()
        {
            View.Unbind();

            if (_equipmentViewData != null)
            {
                _equipmentViewData.Changed -= OnEquipmentChanged;
                _equipmentViewData = null;
            }
        }

        protected override void OnRender()
        {
            View.RequestRender(_equipmentViewData);
        }

        private void OnEquipmentChanged()
        {
            Render();
        }
    }
}
