namespace Assets._Game.Scripts.UI.Windows.Controllers.ItemPreview
{
    public sealed class ItemStacksPreviewWindowController : WindowControllerBase<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments>
    {
        private ItemStacksPreviewWindow _window;
        private IItemStacksPreviewStrategy _strategy;

        public override void Initialize(ItemStacksPreviewWindowControllerArguments arguments)
        {
            base.Initialize(arguments);

            _strategy = arguments.Strategy;
            if (_strategy == null)
            {
                throw new System.ArgumentException("Strategy must be provided");
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
        public readonly IItemStacksPreviewStrategy Strategy;

        public ItemStacksPreviewWindowControllerArguments(IItemStacksPreviewStrategy strategy)
        {
            Strategy = strategy;
        }
    }
}