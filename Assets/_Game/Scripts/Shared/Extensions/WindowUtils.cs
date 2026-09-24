using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Systems;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;
using System;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class WindowUtils
    {
        public static void ShowAmountPicker(IGlobalEventBus globalEventBus, int minAmount, int maxAmount, Action<int> onAmountSelected)
        {
            globalEventBus.Publish(
                new WindowOpenRequest(
                    WindowId.AmountPicker,
                    new AmountPickerWindowControllerArguments(
                        minAmount,
                        maxAmount,
                        amount =>
                        {
                            onAmountSelected?.Invoke(amount);
                        })));
        }

        public static void ShowAmountPickerIfNeeded(IGlobalEventBus globalEventBus, int amount, int maxAmount, Action<int> onAmountSelected)
        {
            if (amount > 1)
            {
                ShowAmountPicker(globalEventBus, 1, maxAmount, onAmountSelected);
            }
            else
            {
                onAmountSelected?.Invoke(1);
            }
        }

        public static void ShowConfirmation(IGlobalEventBus globalEventBus, string title, string message, Action<bool> onDecision)
        {
            globalEventBus.Publish(
                new WindowOpenRequest(
                    WindowId.Confirmation,
                    new ConfirmationWindowControllerArguments(
                        title,
                        message,
                        confirmed =>
                        {
                            onDecision?.Invoke(confirmed);
                        })));
        }

        public static void ShowConfirmationOrAmountPicker(IGlobalEventBus globalEventBus, int amount, int maxAmount, string title, string message, Action<int> onAmountSelected)
        {
            if (amount == 1)
            {
                ShowConfirmation(globalEventBus, title, message, confirmed =>
                {
                    if (confirmed)
                    {
                        onAmountSelected?.Invoke(1);
                    }
                });
            }
            else
            {
                ShowAmountPicker(globalEventBus, 1, maxAmount, amount =>
                {
                    onAmountSelected?.Invoke(amount);
                });
            }
        }

        public static void ShowAmountPickerThenConfirmation(IGlobalEventBus globalEventBus, int amount, int maxAmount, string title, Func<int, string> messageBuilder, Action<int> onConfirmed)
        {
            ShowAmountPickerIfNeeded(globalEventBus, amount, maxAmount, selectedAmount =>
            {
                var message = messageBuilder(selectedAmount);
                ShowConfirmation(globalEventBus, title, message, confirmed =>
                {
                    if (confirmed)
                    {
                        onConfirmed?.Invoke(selectedAmount);
                    }
                });
            });
        }
    }
}
