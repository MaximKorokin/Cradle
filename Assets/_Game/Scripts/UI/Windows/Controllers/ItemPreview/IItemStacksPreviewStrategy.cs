namespace Assets._Game.Scripts.UI.Windows.Controllers.ItemPreview
{
    public interface IItemStacksPreviewStrategy
    {
        void Initialize(ItemStacksPreviewWindow window);
        void Cleanup(ItemStacksPreviewWindow window);
        void Redraw(ItemStacksPreviewWindow window);
        void ProcessAction(ItemStackActionType actionType);
    }
}
