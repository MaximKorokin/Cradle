using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public class StorageWindow : UIWindowBase
    {
        [field: SerializeField]
        public InventoryView StorageInventoryView { get; private set; }
    }
}
