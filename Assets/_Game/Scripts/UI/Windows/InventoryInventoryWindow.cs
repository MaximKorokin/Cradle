using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryInventoryWindow : UIWindow
    {
        [SerializeField]
        private InventoryView _firstInventoryView;
        [SerializeField]
        private InventoryView _secondInventoryView;
        [SerializeField]
        private InventorySlotView _inventorySlotTemplate;

        public override void OnShow()
        {
            _inventorySlotTemplate.gameObject.SetActive(false);
            // _controller events +
        }

        public override void OnHide()
        {
            // _controller events -
        }

        public virtual void Render(InventoryModel firstInventory, InventoryModel secondInventory)
        {
            base.Render();

            _firstInventoryView.Render(firstInventory);
            _secondInventoryView.Render(secondInventory);
        }
    }
}
