using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.UI.DataFormatters;

namespace Assets._Game.Scripts.UI.Windows.Controllers.ItemPreview
{
    public sealed class ItemStacksPreviewWindowController : WindowControllerBase<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments>
    {
        private readonly WindowManager _windowManager;
        private readonly IPlayerProvider _playerProvider;
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly ItemStackFormatter _itemStackFormatter;
        private readonly ItemDefinitionFormatter _itemDefinitionFormatter;

        private ItemStacksPreviewWindow _window;
        private IItemStacksPreviewStrategy _strategy;

        public ItemStacksPreviewWindowController(
            WindowManager windowManager,
            IPlayerProvider playerProvider,
            ItemContainerResolver itemContainerResolver,
            ItemStackFormatter itemStackFormatter,
            ItemDefinitionFormatter itemDefinitionFormatter)
        {
            _windowManager = windowManager;
            _playerProvider = playerProvider;
            _itemContainerResolver = itemContainerResolver;
            _itemStackFormatter = itemStackFormatter;
            _itemDefinitionFormatter = itemDefinitionFormatter;
        }

        public override void Initialize(ItemStacksPreviewWindowControllerArguments arguments)
        {
            base.Initialize(arguments);

            _strategy = arguments.CustomStrategy;
            if (_strategy == null)
            {
                throw new System.ArgumentException("CustomStrategy must be provided");
            }
        }

        public override void Bind(ItemStacksPreviewWindow window)
        {
            _window = window;
            _window.ActionButtonClicked += ProcessAction;

            _strategy.Initialize(_window);
            _strategy.Redraw(_window);
        }

        public override void Unbind()
        {
            _strategy.Cleanup(_window);
            _window.ActionButtonClicked -= ProcessAction;
        }

        private void ProcessAction(ItemStackActionType actionType)
        {
            _strategy.ProcessAction(actionType);
        }
    }

    public sealed class ItemStackAction
    {
        public ItemStackActionType Type;
        public string Title;

        public ItemStackAction(ItemStackActionType type, string title)
        {
            Type = type;
            Title = title;
        }
    }

    public enum ItemStackActionType
    {
        Drop,
        Transfer,
        Equip,
        Unequip,
        Use,
        Buy,
        Sell,
    }

    public readonly struct ItemStacksPreviewWindowControllerArguments : IWindowControllerArguments
    {
        public readonly IItemStacksPreviewStrategy CustomStrategy;

        public ItemStacksPreviewWindowControllerArguments(IItemStacksPreviewStrategy customStrategy)
        {
            CustomStrategy = customStrategy;
        }
    }
}