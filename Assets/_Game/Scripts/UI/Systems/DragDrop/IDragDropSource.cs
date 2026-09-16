using UnityEngine;

namespace Assets._Game.Scripts.UI.Systems.DragDrop
{
    public interface IDragDropSource
    {
        bool CanStartDrag();
        RectTransform CreateDragDropVisual();
    }
}
