using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.Views.Widgets;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;

namespace Assets._Game.Scripts.UI.Systems.Click
{
    public class ClickHandlerService
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly WindowControllerArgumentsProvider _controllerArgumentsProvider;

        public ClickHandlerService(
            IGlobalEventBus globalEventBus,
            WindowControllerArgumentsProvider controllerArgumentsProvider)
        {
            _globalEventBus = globalEventBus;
            _controllerArgumentsProvider = controllerArgumentsProvider;
        }

        public void Handle(IClickTarget clickTarget)
        {
            if (clickTarget is WindowOpenTrigger windowOpenTrigger)
            {
                var arguments = _controllerArgumentsProvider.GetPlayerArguments(windowOpenTrigger.WindowId);
                _globalEventBus.Publish(new WindowToggleRequest(windowOpenTrigger.WindowId, arguments));
            }
            else if (clickTarget is ItemStackWidget itemStackWidget && itemStackWidget.ContainsData)
            {
                var arguments = new ItemStacksPreviewWindowControllerArguments(itemStackWidget.ContainerPath, itemStackWidget.GetItemSlotIndex());
                _globalEventBus.Publish(new WindowToggleRequest(WindowId.ItemStacksPreview, arguments));
            }
            else if (clickTarget is DropArea dropArea)
            {
                if (dropArea.Type == DropAreaType.ItemEnchant)
                {
                    var arguments = new ItemEnchantingWindowControllerArguments(null, default, -1);
                    _globalEventBus.Publish(new WindowToggleRequest(WindowId.ItemEnchanting, arguments));
                }
            }
        }
    }
}
