using UnityEngine;
using UnityEngine.UI;

namespace Assets.CoreScripts
{
    [ExecuteAlways]
    public class SafeAreaLimiter : MonoBehaviourBase
    {
        [SerializeField]
        private CanvasScaler _canvasScaler;

        private Rect _safeArea;
        private RectTransform _rectTransform;

        protected override void Awake()
        {
            base.Awake();

            _rectTransform = GetRequiredComponent<RectTransform>();
            if (_canvasScaler == null)
            {
                SLog.Error($"{nameof(_canvasScaler)} in {nameof(SafeAreaLimiter)} is null");
            }
        }

        protected override void Update()
        {
            base.Update();
            if (_safeArea != Screen.safeArea && _canvasScaler != null)
            {
                _safeArea = Screen.safeArea;
                LimitSafeArea();
            }
        }

        private void LimitSafeArea()
        {
            var scaleVector = _canvasScaler.referenceResolution / new Vector2(Screen.width, Screen.height);
            var safePos = _safeArea.position * scaleVector;
            var safeSize = _safeArea.size * scaleVector;

            _rectTransform.offsetMin = safePos;
            _rectTransform.offsetMax = safePos -
                new Vector2(_canvasScaler.referenceResolution.x - safeSize.x, _canvasScaler.referenceResolution.y - safeSize.y);
        }
    }
}