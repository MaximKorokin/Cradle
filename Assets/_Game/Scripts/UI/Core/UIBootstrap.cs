using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.UI.Windows;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public sealed class UIBootstrap : IStartable
    {
        readonly WindowManager _windows;
        readonly InventoryWindow _inventoryPrefab;
        readonly InventoryController _inventory;

        public UIBootstrap(WindowManager windows, InventoryWindow inventoryPrefab, InventoryController inventory)
        {
            _windows = windows;
            _inventoryPrefab = inventoryPrefab;
            _inventory = inventory;
        }

        public void Start()
        {
            var w = _windows.Show(_inventoryPrefab);
            w.Init(_inventory);
        }
    }
}
