using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Views;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class ClickEffectUISystem : UISystemBase
    {
        private PlayerClickInputReader _playerClickInputReader;
        private ClickEffectView _clickEffectView;
        private Canvas _canvas;

        [Inject]
        private void Construct(IGlobalEventBus globalEventBus, ClickEffectView clickEffectView, PlayerClickInputReader playerClickInputReader)
        {
            BaseConstruct(globalEventBus);

            _playerClickInputReader = playerClickInputReader;
            _clickEffectView = clickEffectView;
            _canvas = _clickEffectView.GetComponentInParent<Canvas>();

            TrackGlobalEvent<ClickEffectRequest>(OnClickEffectRequested);
        }

        private void OnClickEffectRequested(ClickEffectRequest e)
        {
            if (IsPointerOverPlayerClickInputReader(e.ScreenPosition)) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.GetComponent<RectTransform>(),
                e.ScreenPosition,
                _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
                out var localPoint);

            _clickEffectView.GetComponent<RectTransform>().localPosition = localPoint;
            _clickEffectView.Play();
        }

        private bool IsPointerOverPlayerClickInputReader(Vector2 screenPosition)
        {
            var results = new List<RaycastResult>();
            var pointerEventData = new PointerEventData(EventSystem.current) { position = screenPosition };
            EventSystem.current.RaycastAll(pointerEventData, results);
            return results.Count == 1 && results[0].gameObject == _playerClickInputReader.gameObject;
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
