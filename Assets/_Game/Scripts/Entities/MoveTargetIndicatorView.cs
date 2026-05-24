using UnityEngine;

namespace Assets._Game.Scripts.Entities
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public sealed class MoveTargetIndicatorView : MonoBehaviour
    {
        [SerializeField] private float _holdDuration = 0.3f;
        [SerializeField] private float _fadeOutDuration = 0.2f;

        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private float _elapsed;
        private float _totalDuration;
        private bool _playing;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();

            _totalDuration = _holdDuration + _fadeOutDuration;
        }

        public void PlayAt(Vector2 worldPosition)
        {
            transform.position = worldPosition;
            _elapsed = 0f;
            _playing = true;
            gameObject.SetActive(true);
            _animator.SetTrigger("ToPlay");
        }

        private void Update()
        {
            if (!_playing)
                return;

            _elapsed += Time.deltaTime;

            if (_elapsed <= _holdDuration)
            {
                SetAlpha(1f);
            }
            else if (_elapsed <= _totalDuration)
            {
                var t = (_elapsed - _holdDuration) / _fadeOutDuration;
                SetAlpha(Mathf.Lerp(1f, 0f, t));
            }
            else
            {
                _playing = false;
                gameObject.SetActive(false);
            }
        }

        private void SetAlpha(float alpha)
        {
            var color = _spriteRenderer.color;
            color.a = alpha;
            _spriteRenderer.color = color;
        }
    }
}
