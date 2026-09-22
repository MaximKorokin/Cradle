using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Windows.Controllers;
using System;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class WindowControllerArgumentsProvider
    {
        private readonly IPlayerProvider _playerProvider;

        public WindowControllerArgumentsProvider(IPlayerProvider playerProvider)
        {
            _playerProvider = playerProvider;
        }

        public IWindowControllerArguments GetPlayerArguments(WindowId windowId)
        {
            var playerId = _playerProvider.ObservablePlayerId;

            return windowId switch
            {
                WindowId.Inventory => new InventoryWindowControllerArguments(playerId),
                WindowId.Equipment => new EquipmentWindowControllerArguments(playerId),
                WindowId.ItemUseSettings => new ItemUseSettingsWindowControllerArguments(playerId),
                WindowId.Quests => new QuestsWindowControllerArguments(playerId),
                WindowId.Stats => new StatsWindowControllerArguments(playerId),
                WindowId.Cheats => new CheatsWindowControllerArguments(playerId, playerId),
                WindowId.LocationTransitionList => new LocationTransitionListWindowControllerArguments(playerId),
                _ => throw new ArgumentException($"Cannot provide default Player arguments for Window with id: {windowId}")
            };
        }
    }
}
