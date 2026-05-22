using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    [RequireComponent(typeof(Image))]
    public sealed class ClickEffectView : MonoBehaviour
    {
        [SerializeField]
        private float _duration = 0.4f;
        [SerializeField]
        private float _maxScale = 1.5f;

        private Image _image;
        private float _elapsed;
        private bool _playing;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.enabled = false;
        }

        public void Play()
        {
            _elapsed = 0f;
            _playing = true;
            _image.enabled = true;
        }

        private void Update()
        {
            if (!_playing)
                return;

            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / _duration);

            transform.localScale = Vector3.one * Mathf.Lerp(0.1f, _maxScale, t);
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, Mathf.Lerp(1f, 0f, t));

            if (t >= 1f)
                _image.enabled = false;
        }
    }
}
