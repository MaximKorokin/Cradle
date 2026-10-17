using Assets._Game.Scripts.UI.Systems.Click;
using Assets._Game.Scripts.UI.Systems.DragDrop;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Common
{
    public sealed class DropArea : MonoBehaviour, IDragDropTarget, IClickTarget
    {
        [SerializeField]
        private DropAreaType _type;
        [SerializeField]
        private Image _highlightImage;

        public DropAreaType Type => _type;

        public bool CanDrop() => true;

        public void SetDragDropHighlight(bool highlighted)
        {
            _highlightImage.enabled = highlighted;
        }
    }

    public enum DropAreaType
    {
        ItemDestroy,
        ItemEnchant,
    }
}
