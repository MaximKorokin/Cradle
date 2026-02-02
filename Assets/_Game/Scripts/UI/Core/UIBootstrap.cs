using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Windows;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public sealed class UIBootstrap : IStartable
    {
        readonly WindowManager _windows;
        readonly InventoryEquipmentWindow _inventoryPrefab;
        readonly GameContext _gameContext;

        public UIBootstrap(
            WindowManager windows,
            InventoryEquipmentWindow inventoryPrefab,
            GameContext gameContext)
        {
            _windows = windows;
            _inventoryPrefab = inventoryPrefab;
            _gameContext = gameContext;
        }

        public void Start()
        {
            var w = _windows.ShowInventoryEquipmentWindow(_inventoryPrefab, _gameContext.IEModule.Inventory, _gameContext.IEModule.Equipment, new());
        }
    }
}
