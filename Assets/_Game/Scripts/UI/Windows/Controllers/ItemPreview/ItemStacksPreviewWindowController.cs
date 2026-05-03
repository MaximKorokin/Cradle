using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
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

        public override void Bind(ItemStacksPreviewWindow window)
        {
            _window = window;
            _window.ActionButtonClicked += ProcessAction;
        }

        public override void Initialize(ItemStacksPreviewWindowControllerArguments arguments)
        {
            base.Initialize(arguments);

            _strategy = CreateStrategy(arguments);
            _strategy.Initialize(_window);
            _strategy.Redraw(_window);
        }

        public override void Unbind()
        {
            _strategy.Cleanup(_window);
            _window.ActionButtonClicked -= ProcessAction;
        }

        private IItemStacksPreviewStrategy CreateStrategy(ItemStacksPreviewWindowControllerArguments arguments)
        {
            return arguments.Mode switch
            {
                ItemStacksPreviewMode.Container => new ContainerItemStacksPreviewStrategy(
                    _windowManager,
                    _playerProvider,
                    _itemContainerResolver,
                    _itemStackFormatter,
                    arguments.EquipmentSlot,
                    arguments.PrimaryContainerSlot,
                    arguments.PrimaryContainerId,
                    arguments.SecondaryContainerId),

                ItemStacksPreviewMode.Definition => new DefinitionItemStacksPreviewStrategy(
                    arguments.ItemDefinition,
                    _itemDefinitionFormatter),

                _ => throw new System.ArgumentException(nameof(arguments.Mode))
            };
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
    }
}
