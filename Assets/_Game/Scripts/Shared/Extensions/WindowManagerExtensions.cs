using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;
using Assets._Game.Scripts.UI.Windows.Controllers.ItemPreview;
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

        public static ItemStacksPreviewWindow ShowItemDefinitionPreview(this WindowManager windowManager, ItemDefinition itemDefinition)
        {
            return windowManager.InstantiateWindow<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments>(
                new ItemStacksPreviewWindowControllerArguments(itemDefinition));
        }
    }
}
