using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Windows;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public sealed class UIBootstrap : IStartable
    {
        readonly WindowManager _windows;
        readonly InventoryWindow _inventoryPrefab;
        readonly GameContext _gameContext;

        public UIBootstrap(
            WindowManager windows,
            InventoryWindow inventoryPrefab,
            GameContext gameContext)
        {
            _windows = windows;
            _inventoryPrefab = inventoryPrefab;
            _gameContext = gameContext;
        }

        public void Start()
        {
            var w = _windows.Show(_inventoryPrefab);
            w.Init(_gameContext.IEController);
        }
    }
}
