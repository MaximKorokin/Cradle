using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.UI.Windows;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public sealed class UIBootstrap : IStartable
    {
        readonly WindowManager _windows;
        readonly InventoryWindow _inventoryPrefab;
        readonly InventoryEquipmentController _inventoryController;

        public UIBootstrap(WindowManager windows, InventoryWindow inventoryPrefab, InventoryEquipmentController inventory)
        {
            _windows = windows;
            _inventoryPrefab = inventoryPrefab;
            _inventoryController = inventory;
        }

        public void Start()
        {
            var w = _windows.Show(_inventoryPrefab);
            w.Init(_inventoryController);
        }
    }
}
