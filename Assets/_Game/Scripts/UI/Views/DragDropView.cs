using Assets._Game.Scripts.Shared.Extensions;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class DragDropView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _nestedVisualParent;

        private RectTransform _currentVisual;

        public void AttachVisual(RectTransform visual)
        {
            if (visual == null) return;

            ClearVisual();

            visual.DisableRaycastTargets();
            visual.SetParent(_nestedVisualParent, false);
            _currentVisual = visual;

            _nestedVisualParent.gameObject.SetActive(true);
        }

        public void ClearVisual()
        {
            if (_currentVisual != null)
            {
                Destroy(_currentVisual.gameObject);
                _currentVisual = null;
            }

            _nestedVisualParent.gameObject.SetActive(false);
        }
    }
}
