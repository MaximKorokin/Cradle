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
                    var tooltipPosition = GetTooltipPosition(((Component)tooltipSource).transform as RectTransform, _currentTooltipContent);
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

        private static Vector2 GetTooltipPosition(RectTransform sourceTransform, RectTransform tooltipContent)
        {
            var tooltipSize = new Vector2(tooltipContent.rect.width, tooltipContent.rect.height);
            var sourceRect = sourceTransform.rect;
            var sourceTopLeft = sourceTransform.TransformPoint(new Vector3(sourceRect.xMin, sourceRect.yMax));
            var sourceTopRight = sourceTransform.TransformPoint(new Vector3(sourceRect.xMax, sourceRect.yMax));
            var halfTooltipSize = tooltipSize / 2;

            // Try to position the tooltip above the source element
            var abovePosition = (Vector2)sourceTopLeft + new Vector2(halfTooltipSize.x, halfTooltipSize.y);
            if (FitsInHeight(abovePosition, tooltipSize))
            {
                // Clamp the x position to ensure the tooltip stays within screen bounds
                abovePosition.x = Mathf.Clamp(abovePosition.x, halfTooltipSize.x, Screen.width - halfTooltipSize.x);
                return abovePosition;
            }

            // Try to position the tooltip to the right of the source element
            var topAlignedY = Screen.height - halfTooltipSize.y;
            var rightPosition = new Vector2(sourceTopRight.x + halfTooltipSize.x, topAlignedY);
            if (FitsOnScreen(rightPosition, tooltipSize))
            {
                return rightPosition;
            }

            // Try to position the tooltip to the left of the source element
            var leftPosition = new Vector2(sourceTopLeft.x - halfTooltipSize.x, topAlignedY);
            leftPosition.x = Mathf.Clamp(leftPosition.x, halfTooltipSize.x, Screen.width - halfTooltipSize.x);
            return leftPosition;
        }

        private static bool FitsOnScreen(Vector2 position, Vector2 size)
        {
            var halfSize = size / 2;
            return position.x - halfSize.x >= 0 &&
                   position.x + halfSize.x <= Screen.width &&
                   FitsInHeight(position, size);
        }

        private static bool FitsInHeight(Vector2 position, Vector2 size)
        {
            var halfHeight = size.y / 2;
            return position.y - halfHeight >= 0 &&
                   position.y + halfHeight <= Screen.height;
        }
    }
}
