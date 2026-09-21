namespace Assets._Game.Scripts.UI.Windows.Controllers.ItemPreview
{
    public sealed class ItemStacksPreviewWindowController : WindowControllerBase<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments>
    {
        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (Arguments.Strategy == null)
            {
                throw new System.ArgumentException("Strategy must be provided");
            }
        }

        protected override void OnBind()
        {
            base.OnBind();

            Window.ActionButtonClicked += ProcessAction;
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            Arguments.Strategy.Cleanup(Window);
            Window.ActionButtonClicked -= ProcessAction;
        }

        private void ProcessAction(ItemStackActionType actionType)
        {
            Arguments.Strategy.ProcessAction(actionType);
        }

        protected override void Redraw()
        {
            Arguments.Strategy.Initialize(Window);
            Arguments.Strategy.Redraw(Window);
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
        Destroy,
        Drop,
        Transfer,
        Equip,
        Unequip,
        Use,
        Buy,
        Sell,
        Enchant,
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