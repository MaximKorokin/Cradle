using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryEquipmentWindow : UIWindowBase
    {
        [SerializeField]
        private InventoryView _inventoryView;
        [SerializeField]
        private EquipmentView _equipmentView;

        public InventoryView InventoryView => _inventoryView;
        public EquipmentView EquipmentView => _equipmentView;

        public override void OnHide()
        {
            base.OnHide();

            _inventoryView.Unbind();
            _equipmentView.Unbind();
        }

        public void Render(IInventoryHudData inventoryHudData, IEquipmentHudData equipmentHudData)
        {
            _inventoryView.Render(inventoryHudData);
            _equipmentView.Render(equipmentHudData);
            _equipmentView.Bind();
        }
    }
}
