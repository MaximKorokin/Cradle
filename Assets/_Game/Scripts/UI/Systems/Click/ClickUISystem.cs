using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Shared.Extensions;
using System;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems.Click
{
    public class ClickUISystem : UISystemBase
    {
        private const int ClickThreshold = 20;

        private ClickHandlerService _clickHandlerService;

        private IClickTarget _currentClickTarget;

        [Inject]
        private void Construct(
            IGlobalEventBus globalEventBus,
            ClickHandlerService clickHandlerService)
        {
            BaseConstruct(globalEventBus);

            _clickHandlerService = clickHandlerService;

            TrackGlobalEvent<PointerDownEvent>(OnPointerDown);
            TrackGlobalEvent<PointerUpEvent>(OnPointerUp);
            TrackGlobalEvent<PointerMoveEvent>(OnPointerMove);
        }

        private void OnPointerDown(PointerDownEvent e)
        {
            if (e.Context.IsOverUI)
            {
                GlobalEventBus.Publish(new ClickEffectRequest(e.Context.ScreenPosition));
            }

            if (e.Context.UnderlyingElement.TryGetComponentInParent<IClickTarget>(out var clickTarget))
            {
                _currentClickTarget = clickTarget;
            }
        }

        private void OnPointerUp(PointerUpEvent e)
        {
            if (e.Context.UnderlyingElement.TryGetComponentInParent<IClickTarget>(out var clickTarget) &&
                _currentClickTarget == clickTarget)
            {
                _clickHandlerService.Handle(_currentClickTarget);
            }
        }

        private void OnPointerMove(PointerMoveEvent e)
        {

        }
    }
}
