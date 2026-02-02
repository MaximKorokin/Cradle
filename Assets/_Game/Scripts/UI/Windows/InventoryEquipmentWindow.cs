using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryEquipmentWindow : UIWindow
    {
        [SerializeField]
        private InventoryView _inventoryView;
        [SerializeField]
        private EquipmentView _equipmentView;

        public override void OnShow()
        {
            // _controller events +
        }

        public override void OnHide()
        {
            // _controller events -
        }

        public virtual void Render(InventoryModel inventoryModel, EquipmentModel equipmentModel)
        {
            base.Render();

            _inventoryView.Render(inventoryModel);
            _equipmentView.Render(equipmentModel);
        }
    }
}
