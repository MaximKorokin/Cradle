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
            Clear();

            _inventorySlotTemplate.gameObject.SetActive(false);
            foreach (var (_, stack) in inventoryModel.Enumerate())
            {
                var slotView = Instantiate(_inventorySlotTemplate, _inventorySlotsParent);
                slotView.gameObject.SetActive(true);
                if (slotView != null)
                {
                    slotView.Render(stack);
                }
            }
        }

        public void Clear()
        {
            foreach (Transform child in _inventorySlotsParent)
            {
                if (child.gameObject == _inventorySlotTemplate.gameObject)
                    continue;
                Destroy(child.gameObject);
            }
        }
    }
}
