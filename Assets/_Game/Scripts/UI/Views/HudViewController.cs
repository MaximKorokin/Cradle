using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.UI.Windows;
using System;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class HudViewController : IDisposable
    {
        private readonly HudView _hudView;
        private readonly WindowManager _windowManager;
        private readonly PlayerContext _playerContext;
        private readonly ItemDefinitionCatalog _itemDefinitionCatalog;
        private readonly ItemStackAssembler _itemStackAssembler;

        public HudViewController(
            HudView hudView,
            WindowManager windowManager,
            ItemDefinitionCatalog itemDefinitionCatalog,
            ItemStackAssembler itemStackAssembler,
            PlayerContext playerContext)
        {
            _hudView = hudView;
            _windowManager = windowManager;
            _playerContext = playerContext;
            _itemDefinitionCatalog = itemDefinitionCatalog;
            _itemStackAssembler = itemStackAssembler;

            _hudView.InventoryButtonClicked += OnInventoryButtonClicked;
            _hudView.StashButtonClicked += OnStashButtonClicked;
            _hudView.StatsButtonClicked += OnStatsButtonClicked;
            _hudView.CheatsButtonClicked += OnCheatsButtonClicked;
        }

        public void Dispose()
        {
            _hudView.InventoryButtonClicked -= OnInventoryButtonClicked;
            _hudView.StashButtonClicked -= OnStashButtonClicked;
            _hudView.StatsButtonClicked -= OnStatsButtonClicked;
            _hudView.CheatsButtonClicked -= OnCheatsButtonClicked;
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

        private void OnCheatsButtonClicked()
        {
            _windowManager.ShowCheatsWindow(_itemDefinitionCatalog, _itemStackAssembler, _playerContext);
        }

        private void OnStatsButtonClicked()
        {
            _windowManager.ShowStatsWindow(_playerContext.StatsModule);
        }
    }
}
