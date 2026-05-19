namespace Assets._Game.Scripts.UI.Windows.Controllers.ItemPreview
{
    public sealed class ItemStacksPreviewWindowController : WindowControllerBase<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments>
    {
        private ItemStacksPreviewWindow _window;

        public override void Initialize(ItemStacksPreviewWindowControllerArguments arguments)
        {
            base.Initialize(arguments);

            if (Arguments.Strategy == null)
            {
                throw new System.ArgumentException("Strategy must be provided");
            }
        }

        public override void Bind(ItemStacksPreviewWindow window)
        {
            _window = window;
            _window.ActionButtonClicked += ProcessAction;

            Arguments.Strategy.Initialize(_window);
            Arguments.Strategy.Redraw(_window);
        }

        public override void Unbind()
        {
            Arguments.Strategy.Cleanup(_window);
            _window.ActionButtonClicked -= ProcessAction;
        }

        private void ProcessAction(ItemStackActionType actionType)
        {
            Arguments.Strategy.ProcessAction(actionType);
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