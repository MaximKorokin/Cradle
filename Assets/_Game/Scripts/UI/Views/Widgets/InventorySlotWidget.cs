using UnityEngine;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public sealed class InventorySlotWidget : MonoBehaviour
    {
        [SerializeField]
        private ItemStackWidget _itemStackWidget;

        public ItemStackWidget SlotView => _itemStackWidget;
    }
}
