using UnityEngine;

namespace Assets._Game.Scripts.UI.Systems.DragDrop
{
    public interface IDragDropSource
    {
        RectTransform CreateDragDropVisual();
    }
}
