using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.Views.Widgets;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Systems.Tooltip
{
    public sealed class TooltipHandlerService
    {
        private readonly IGlobalEventBus _globalEventBus;

        public TooltipHandlerService(IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;
        }

        public (RectTransform Content, Action CleanAction) Handle(ITooltipSource tooltipSource)
        {
            if (tooltipSource is ContainerSlotWidget containerSlot && containerSlot.ContainsData)
            {
                var (window, closeAction) = WindowUtils.ShowWindowWithCloseAction(
                    _globalEventBus,
                    WindowId.ItemStacksPreview,
                    new ItemStacksPreviewWindowControllerArguments(containerSlot.ContainerPath, containerSlot.SlotIndex));

                return (window.transform as RectTransform, closeAction);
            }

            return (null, null);
        }
    }
}
