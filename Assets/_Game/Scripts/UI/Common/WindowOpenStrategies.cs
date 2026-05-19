using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;

namespace Assets._Game.Scripts.UI.Common
{
    public interface IWindowOpenStrategy
    {
        UIWindowBase Open(string entityId);
    }

    public abstract class WindowOpenStrategy : IWindowOpenStrategy
    {
        protected readonly WindowManager WindowManager;

        protected WindowOpenStrategy(WindowManager windowManager)
        {
            WindowManager = windowManager;
        }

        public abstract UIWindowBase Open(string entityId);
    }

    public abstract class PlayerWindowOpenStrategy : WindowOpenStrategy
    {
        private readonly IPlayerProvider _playerProvider;

        protected PlayerWindowOpenStrategy(WindowManager windowManager, IPlayerProvider playerProvider)
            : base(windowManager)
        {
            _playerProvider = playerProvider;
        }

        public UIWindowBase Open() => Open(_playerProvider.Player.Id);
    }

    public sealed class InventoryEquipmentWindowOpenStrategy : PlayerWindowOpenStrategy
    {
        public InventoryEquipmentWindowOpenStrategy(WindowManager windowManager, IPlayerProvider playerProvider)
            : base(windowManager, playerProvider) { }

        public override UIWindowBase Open(string entityId)
        {
            return WindowManager.InstantiateWindow<InventoryEquipmentWindow, InventoryEquipmentWindowControllerArguments>(
                new(entityId, entityId));
        }
    }

    public sealed class ItemUseSettingsWindowOpenStrategy : PlayerWindowOpenStrategy
    {
        public ItemUseSettingsWindowOpenStrategy(WindowManager windowManager, IPlayerProvider playerProvider)
            : base(windowManager, playerProvider) { }

        public override UIWindowBase Open(string entityId)
        {
            return WindowManager.InstantiateWindow<ItemUseSettingsWindow, ItemUseSettingsWindowControllerArguments>(
                new(entityId));
        }
    }

    public sealed class CheatsWindowOpenStrategy : PlayerWindowOpenStrategy
    {
        public CheatsWindowOpenStrategy(WindowManager windowManager, IPlayerProvider playerProvider)
            : base(windowManager, playerProvider) { }

        public override UIWindowBase Open(string entityId)
        {
            return WindowManager.InstantiateWindow<CheatsWindow, CheatsWindowControllerArguments>(
                new(entityId, entityId));
        }
    }

    public sealed class QuestsWindowOpenStrategy : PlayerWindowOpenStrategy
    {
        public QuestsWindowOpenStrategy(WindowManager windowManager, IPlayerProvider playerProvider)
            : base(windowManager, playerProvider) { }

        public override UIWindowBase Open(string entityId)
        {
            return WindowManager.InstantiateWindow<QuestsWindow, QuestsWindowControllerArguments>(
                new(entityId));
        }
    }
}
