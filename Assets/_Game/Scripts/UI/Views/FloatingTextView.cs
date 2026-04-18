using Assets._Game.Scripts.Infrastructure.Configs;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class FloatingTextView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;
        [Header("Animation")]
        [SerializeField]
        private AnimationCurve _scaleCurve;
        [SerializeField]
        private float _fadeTime;
        [SerializeField]
        private Vector2 _fadeVelocity;

        private float _initialLifetime;
        private float _lifetime;

        public void Bind(string text, FloatingTextStyle style)
        {
            _initialLifetime = style.Duration;
            _lifetime = style.Duration;

            _text.text = text;
            _text.color = style.Color;
            _text.fontSize *= style.SizeScale;

            var rectTransform = transform as RectTransform;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x * style.SizeScale, rectTransform.sizeDelta.y * style.SizeScale);
        }

        private void Update()
        {
            _lifetime -= Time.deltaTime;
            if (_lifetime < 0)
            {
                Destroy(gameObject);
                return;
            }
            RenderTick();
        }

        private void RenderTick()
        {
            var lifetimePercent = 1 - _lifetime / _initialLifetime;
            var showPercent = _scaleCurve.Evaluate(lifetimePercent);
            transform.localScale = Vector3.one * showPercent;
            if (_lifetime < _fadeTime)
            {
                var fadePercent = 1 - _lifetime / _fadeTime;
                _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 1 - fadePercent);
                transform.position += (Vector3)_fadeVelocity * Time.deltaTime;
            }
        }
    }
}
