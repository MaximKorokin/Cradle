using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Windows;
using System;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class HudViewController : IDisposable
    {
        private readonly HudView _hudView;
        private readonly WindowManager _windowManager;
        private readonly PlayerContext _playerContext;

        public HudViewController(
            HudView hudView,
            WindowManager windowManager,
            PlayerContext playerContext)
        {
            _windowManager = windowManager;
            _playerContext = playerContext;
            _hudView = hudView;

            _hudView.InventoryButtonClicked += OnInventoryButtonClicked;
            _hudView.StashButtonClicked += OnStashButtonClicked;
        }

        public void Dispose()
        {
            _hudView.InventoryButtonClicked -= OnInventoryButtonClicked;
            _hudView.StashButtonClicked -= OnStashButtonClicked;
        }

        private void OnInventoryButtonClicked()
        {
            _windowManager.ShowInventoryEquipmentWindow(
                _playerContext.IEModule.Inventory,
                _playerContext.IEModule.Equipment,
                new());
        }

        private void OnStashButtonClicked()
        {
            _windowManager.ShowInventoryInventoryWindow(
                _playerContext.IEModule.Equipment,
                _playerContext.IEModule.Inventory,
                _playerContext.StashInventory,
                new());
        }
    }
}
