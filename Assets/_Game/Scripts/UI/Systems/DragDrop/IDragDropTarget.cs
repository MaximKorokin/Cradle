namespace Assets._Game.Scripts.UI.Systems.DragDrop
{
    public interface IDragDropTarget
    {
        bool CanDrop();
        void SetDragDropHighlight(bool highlighted);
    }
}
