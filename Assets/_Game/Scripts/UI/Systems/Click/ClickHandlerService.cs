using Assets._Game.Scripts.Infrastructure.Game;
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

                // DEBUG
                WindowSettings settings = default;
                if (windowOpenTrigger.WindowId == WindowId.StatusEffectList)
                    settings = new(false, false, false);
                // DEBUG

                _globalEventBus.Publish(new WindowToggleRequest(windowOpenTrigger.WindowId, arguments, settings));
            }
            else if (clickTarget is ContainerSlotWidget containerSlot && containerSlot.ContainsData)
            {
                var arguments = new ItemStacksPreviewWindowControllerArguments(containerSlot.ContainerPath, containerSlot.SlotIndex);
                _globalEventBus.Publish(new WindowToggleRequest(WindowId.ItemStacksPreview, arguments));
            }
        }
    }
}
