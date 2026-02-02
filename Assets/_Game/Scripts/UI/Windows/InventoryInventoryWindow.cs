using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryInventoryWindow : UIWindow
    {
        [SerializeField]
        private RectTransform _firstInventorySlotsParent;
        [SerializeField]
        private RectTransform _secondInventorySlotsParent;
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

            foreach (var (_, stack) in firstInventory.Enumerate())
            {
                var slotView = Instantiate(_inventorySlotTemplate, _firstInventorySlotsParent);
                slotView.gameObject.SetActive(true);
                if (slotView != null)
                {
                    slotView.Render(stack);
                }
            }

            foreach (var (_, stack) in secondInventory.Enumerate())
            {
                var slotView = Instantiate(_inventorySlotTemplate, _secondInventorySlotsParent);
                slotView.gameObject.SetActive(true);
                if (slotView != null)
                {
                    slotView.Render(stack);
                }
            }
        }
    }
}
