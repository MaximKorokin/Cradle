using UnityEngine;

namespace Assets._Game.Scripts.UI.Common
{
    [ExecuteInEditMode]
    public class RectTransformSizeSynchronizer : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _source;
        [SerializeField]
        private RectTransform _target;
        [SerializeField]
        private Vector2 _offset;
        [SerializeField]
        private Vector2 _minSizeLimit = -Vector2.one;
        [SerializeField]
        private Vector2 _maxSizeLimit = -Vector2.one;

        private void LateUpdate()
        {
            if (_source == null || _target == null) return;

            var newSize = _source.sizeDelta + _offset;

            if (_minSizeLimit.x >= 0 && newSize.x < _minSizeLimit.x)
            {
                newSize.x = _minSizeLimit.x;
            }
            if (_maxSizeLimit.x >= 0 && newSize.x > _maxSizeLimit.x)
            {
                newSize.x = _maxSizeLimit.x;
            }

            if (_minSizeLimit.y >= 0 && newSize.y < _minSizeLimit.y)
            {
                newSize.y = _minSizeLimit.y;
            }
            if (_maxSizeLimit.y >= 0 && newSize.y > _maxSizeLimit.y)
            {
                newSize.y = _maxSizeLimit.y;
            }
            _target.sizeDelta = newSize;
        }
    }
}
