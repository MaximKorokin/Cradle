using Assets._Game.Scripts.Items.Inventory;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class InventoryView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _inventorySlotsParent;
        [SerializeField]
        private InventorySlotView _inventorySlotTemplate;

        public void Render(InventoryModel inventoryModel)
        {
            _inventorySlotTemplate.gameObject.SetActive(false);

            foreach (var (index, stack) in inventoryModel.Enumerate())
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
