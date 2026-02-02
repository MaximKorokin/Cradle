using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryEquipmentWindow : UIWindow
    {
        [SerializeField]
        private EquipmentSlotView[] _slots;
        [SerializeField]
        private RectTransform _inventorySlotsParent;
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

        public void Render(InventoryModel inventory, EquipmentModel equipmentModel)
        {
            foreach (var slot in equipmentModel.Enumerate())
            {
                var slotView = System.Array.Find(_slots, s => s.SlotType == slot.index);
                if (slotView != null)
                {
                    slotView.Render(slot.stack);
                }
            }

            foreach (var (index, stack) in inventory.Enumerate())
            {
                var slotView = Instantiate(_inventorySlotTemplate, _inventorySlotsParent);
                slotView.gameObject.SetActive(true);
                if (slotView != null)
                {
                    slotView.Render(stack);
                }
            }
        }
    }
}
