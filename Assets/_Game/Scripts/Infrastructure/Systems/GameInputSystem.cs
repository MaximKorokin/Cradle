using Assets._Game.Scripts.Infrastructure.Game;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class GameInputSystem : SystemBase, ITickSystem
    {
        private Vector2 _previousPointerPosition;
        private bool _isPointerDown;

        public GameInputSystem(IGlobalEventBus globalEventBus) : base(globalEventBus)
        {
        }

        public void Tick(float delta)
        {
            var pointerPosition = Pointer.current?.position.ReadValue() ?? Vector2.zero;
            var pointerContext = new PointerContext(
                pointerPosition,
                _previousPointerPosition,
                _isPointerDown,
                GetUnderlyingElement(pointerPosition));

            if (Pointer.current.press.isPressed && !_isPointerDown)
            {
                _isPointerDown = true;
                GlobalEventBus.Publish(new PointerDownEvent(pointerContext));
            }

            if (!Pointer.current.press.isPressed && _isPointerDown)
            {
                _isPointerDown = false;
                GlobalEventBus.Publish(new PointerUpEvent(pointerContext));
            }

            GlobalEventBus.Publish(new PointerMoveEvent(pointerContext));

            _previousPointerPosition = pointerPosition;
        }

        private GameObject GetUnderlyingElement(Vector2 screenPosition)
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null)
                return null;

            var eventData = new PointerEventData(eventSystem)
            {
                position = screenPosition
            };

            var results = new List<RaycastResult>();
            eventSystem.RaycastAll(eventData, results);

            return results.Count > 0 ? results[0].gameObject : null;
        }
    }

    public readonly struct PointerContext
    {
        public readonly Vector2 ScreenPosition;
        public readonly Vector2 PreviousScreenPosition;
        public readonly bool IsPressed;
        public readonly GameObject UnderlyingElement;
        public readonly bool IsOverUI => UnderlyingElement != null && UnderlyingElement.TryGetComponent<RectTransform>(out _);

        public PointerContext(
            Vector2 screenPosition,
            Vector2 previousScreenPosition,
            bool isPressed,
            GameObject underlyingElement)
        {
            ScreenPosition = screenPosition;
            PreviousScreenPosition = previousScreenPosition;
            IsPressed = isPressed;
            UnderlyingElement = underlyingElement;
        }
    }

    public readonly struct PointerDownEvent : IGlobalEvent
    {
        public readonly PointerContext Context;

        public PointerDownEvent(PointerContext context)
        {
            Context = context;
        }
    }

    public readonly struct PointerUpEvent : IGlobalEvent
    {
        public readonly PointerContext Context;

        public PointerUpEvent(PointerContext context)
        {
            Context = context;
        }
    }

    public readonly struct PointerMoveEvent : IGlobalEvent
    {
        public readonly PointerContext Context;

        public PointerMoveEvent(PointerContext context)
        {
            Context = context;
        }
    }
}
