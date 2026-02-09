using Assets.CoreScripts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.Common
{
    public class SelectableElementsGroup<T> where T : SelectableElement
    {
        private readonly HashSet<T> _selectableElements = new();

        public IEnumerable<T> SelectableElements => _selectableElements;

        public event Action<T> SelectedChanged;

        public void AddSelectable(T selectableElement)
        {
            _selectableElements.Add(selectableElement);
            selectableElement.SelectionChanged += OnSelectableElementSelectionChanged;
        }

        public void RemoveSelectable(T selectableElement)
        {
            _selectableElements.Remove(selectableElement);
            selectableElement.SelectionChanged -= OnSelectableElementSelectionChanged;
        }

        private void OnSelectableElementSelectionChanged(SelectableElement selectableElement, bool selected) => OnSelectableElementSelectionChanged((T)selectableElement, selected);
        private void OnSelectableElementSelectionChanged(T selectableElement, bool selected)
        {
            if (selected)
            {
                _selectableElements.Except(selectableElement.Yield()).ForEach(x => x.SetSelection(false, true));
                SelectedChanged?.Invoke(selectableElement);
            }
            else
            {
                SelectedChanged?.Invoke(null);
            }
        }
    }
}
