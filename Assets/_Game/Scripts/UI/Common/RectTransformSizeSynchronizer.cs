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
        private Vector2 _sizeLimit = -Vector2.one;

        private void Update()
        {
            if (_source == null || _target == null) return;

            var newSize = _source.sizeDelta + _offset;
            if (_sizeLimit.x >= 0 && newSize.x > _sizeLimit.x)
            {
                newSize.x = _sizeLimit.x;
            }
            if (_sizeLimit.y >= 0 && newSize.y > _sizeLimit.y)
            {
                newSize.y = _sizeLimit.y;
            }
            _target.sizeDelta = newSize;
        }
    }
}
