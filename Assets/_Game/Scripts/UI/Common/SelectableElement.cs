using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._Game.Scripts.UI.Common
{
    public abstract class SelectableElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private SelectionType _selectionType;

        private bool _isSelected;

        public event Action<SelectableElement, bool> SelectionChanged;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_selectionType == SelectionType.PointerDown)
            {
                SetSelection(!_isSelected, false);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_selectionType == SelectionType.PointerUp && !eventData.dragging)
            {
                SetSelection(!_isSelected, false);
            }
        }

        public virtual void SetSelection(bool selection, bool silent)
        {
            var previousValue = _isSelected;
            _isSelected = selection;
            if (!silent && previousValue != _isSelected)
            {
                SelectionChanged?.Invoke(this, _isSelected);
            }
        }

        private enum SelectionType
        {
            PointerDown,
            PointerUp,
        }
    }
}
