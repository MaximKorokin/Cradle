using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.Views.Widgets;
using System;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems.Tooltip
{
    public sealed class TooltipUISystem : UISystemBase
    {
        private TooltipWidget _tooltipWidget;
        private TooltipHandlerService _tooltipHandlerService;

        private ITooltipSource _currentTooltipSource;
        private RectTransform _currentTooltipContent;
        private Action _currentTooltipCleanAction;

        [Inject]
        private void Construct(
            IGlobalEventBus globalEventBus,
            TooltipWidget tooltipWidget,
            TooltipHandlerService tooltipHandlerService)
        {
            BaseConstruct(globalEventBus);

            _tooltipWidget = tooltipWidget;
            _tooltipHandlerService = tooltipHandlerService;

            TrackGlobalEvent<PointerMoveEvent>(OnPointerMove);
        }

        private void OnPointerMove(PointerMoveEvent e)
        {
            var foundTooltipSource = e.Context.UnderlyingElement.TryGetComponentInParent<ITooltipSource>(out var tooltipSource);
            _tooltipWidget.SetVisible(foundTooltipSource);
            if (foundTooltipSource)
            {
                if (_currentTooltipContent != null)
                {
                    var tooltipRect = _currentTooltipContent.rect;
                    var tooltipSize = new Vector2(tooltipRect.width, tooltipRect.height);
                    var tooltipPosition = e.Context.ScreenPosition + new Vector2(tooltipSize.x / 2, tooltipSize.y / 2);
                    _tooltipWidget.SetPosition(tooltipPosition);
                }

                if (_currentTooltipSource == tooltipSource) return;
                if (_currentTooltipSource != null) CleanState();

                _currentTooltipSource = tooltipSource;
                var (content, cleanAction) = _tooltipHandlerService.Handle(tooltipSource);

                if (content == null || cleanAction == null) return;

                _currentTooltipContent = content;
                _currentTooltipCleanAction = cleanAction;
                _tooltipWidget.SetContent(_currentTooltipContent);
            }
            else
            {
                CleanState();
            }
        }

        private void CleanState()
        {
            _currentTooltipSource = null;
            _tooltipWidget.SetVisible(false);
            _currentTooltipCleanAction?.Invoke();
            _currentTooltipCleanAction = null;
        }
    }
}
