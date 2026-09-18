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
    }
}
