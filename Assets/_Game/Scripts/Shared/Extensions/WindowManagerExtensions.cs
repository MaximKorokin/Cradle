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
            if (maxAmount > 1 && amount > 1)
            {
                ShowAmountPicker(windowManager, 1, maxAmount, onAmountSelected);
            }
            else
            {
                onAmountSelected?.Invoke(1);
            }
        }
    }
}
