using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._Game.Scripts.UI.Common
{
    public abstract class SelectableElement : MonoBehaviour, IPointerClickHandler
    {
        private bool _isSelected;

        public bool IsSelected => _isSelected;

        public event Action<SelectableElement, bool> SelectionChanged;

        public void OnPointerClick(PointerEventData eventData)
        {
            SetSelection(!_isSelected, false);
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
    }
}
