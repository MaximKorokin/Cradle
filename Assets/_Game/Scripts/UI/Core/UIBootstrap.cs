using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Windows;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public sealed class UIBootstrap : IStartable
    {
        readonly WindowManager _windows;
        readonly GameContext _gameContext;

        public UIBootstrap(
            WindowManager windows,
            GameContext gameContext)
        {
            _windows = windows;
            _gameContext = gameContext;
        }

        public void Start()
        {
            _windows.ShowInventoryEquipmentWindow(
                _gameContext.IEModule.Inventory,
                _gameContext.IEModule.Equipment,
                new());
            //_windows.ShowInventoryInventoryWindow(
            //    _gameContext.IEModule.Inventory,
            //    _gameContext.StashInventory,
            //    new());
        }
    }
}
