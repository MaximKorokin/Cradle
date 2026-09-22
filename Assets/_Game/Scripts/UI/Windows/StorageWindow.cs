using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public class StorageWindow : UIWindowBase
    {
        [SerializeField]
        private InventoryView _storageInventoryView;

        public InventoryView StorageInventoryView => _storageInventoryView;
    }
}
