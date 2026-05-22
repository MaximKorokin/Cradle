using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Views;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class ClickEffectUISystem : UISystemBase
    {
        private ClickEffectView _clickEffectView;
        private Canvas _canvas;

        [Inject]
        private void Construct(IGlobalEventBus globalEventBus, ClickEffectView clickEffectView)
        {
            BaseConstruct(globalEventBus);

            _clickEffectView = clickEffectView;
            _canvas = _clickEffectView.GetComponentInParent<Canvas>();

            TrackGlobalEvent<ClickEffectRequest>(OnClickEffectRequested);
        }

        private void OnClickEffectRequested(ClickEffectRequest e)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.GetComponent<RectTransform>(),
                e.ScreenPosition,
                _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
                out var localPoint);

            _clickEffectView.GetComponent<RectTransform>().localPosition = localPoint;
            _clickEffectView.Play();
        }
    }

    public readonly struct ClickEffectRequest : IGlobalEvent
    {
        public Vector2 ScreenPosition { get; }

        public ClickEffectRequest(Vector2 screenPosition)
        {
            ScreenPosition = screenPosition;
        }
    }
}
