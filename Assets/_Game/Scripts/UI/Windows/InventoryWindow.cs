using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class InventoryWindow : UIWindowBase
    {
        [field: SerializeField]
        public InventoryView InventoryView { get; private set; }
    }
}
