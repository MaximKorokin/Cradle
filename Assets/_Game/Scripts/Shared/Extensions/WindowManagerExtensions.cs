using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;
using System;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class WindowManagerExtensions
    {
        public static void ShowAmountPicker(this WindowManager windowManager, int minAmount, int maxAmount, Action<int> onAmountSelected)
        {
            AmountPickerWindow amountPickerWindow = null;
            amountPickerWindow = windowManager.InstantiateWindow<AmountPickerWindow, AmountPickerWindowControllerArguments>(
                new(minAmount,
                    maxAmount,
                    amount =>
                    {
                        onAmountSelected?.Invoke(amount);
                        windowManager.CloseWindow(amountPickerWindow);
                    }));
        }

        public static void ShowAmountPickerIfNeeded(this WindowManager windowManager, int amount, int maxAmount, Action<int> onAmountSelected)
        {
            if (amount > 1)
            {
                ShowAmountPicker(windowManager, 1, maxAmount, onAmountSelected);
            }
            else
            {
                onAmountSelected?.Invoke(1);
            }
        }

        public static void ShowConfirmation(this WindowManager windowManager, string title, string message, Action<bool> onDecision)
        {
            ConfirmationWindow confirmationWindow = null;
            confirmationWindow = windowManager.InstantiateWindow<ConfirmationWindow, ConfirmationWindowControllerArguments>(
                new(title,
                    message,
                    confirmed =>
                    {
                        onDecision?.Invoke(confirmed);
                        windowManager.CloseWindow(confirmationWindow);
                    }));
        }

        public static void ShowConfirmationOrAmountPicker(this WindowManager windowManager, int amount, int maxAmount, string title, string message, Action<int> onAmountSelected)
        {
            if (amount == 1)
            {
                ShowConfirmation(windowManager, title, message, confirmed =>
                {
                    if (confirmed)
                    {
                        onAmountSelected?.Invoke(1);
                    }
                });
            }
            else
            {
                ShowAmountPicker(windowManager, 1, maxAmount, amount =>
                {
                    onAmountSelected?.Invoke(amount);
                });
            }
        }

        public static void ShowAmountPickerThenConfirmation(this WindowManager windowManager, int amount, int maxAmount, string title, Func<int, string> messageBuilder, Action<int> onConfirmed)
        {
            windowManager.ShowAmountPickerIfNeeded(amount, maxAmount, selectedAmount =>
            {
                var message = messageBuilder(selectedAmount);
                windowManager.ShowConfirmation(title, message, confirmed =>
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
