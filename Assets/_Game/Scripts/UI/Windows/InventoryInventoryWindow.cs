using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryInventoryWindow : UIWindowBase
    {
        [SerializeField]
        private InventoryView _firstInventoryView;
        [SerializeField]
        private InventoryView _secondInventoryView;

        public InventoryView FirstInventoryView => _firstInventoryView;
        public InventoryView SecondInventoryView => _secondInventoryView;
    }
}
