using System;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class AmountPickerWindowController : WindowControllerBase<AmountPickerWindow, AmountPickerWindowControllerArguments>
    {
        private AmountPickerWindow _window;

        public override void Bind(AmountPickerWindow window)
        {
            _window = window;
            _window.Render(Arguments.MinAmount, Arguments.MaxAmount);
            _window.AmountSelected += OnAmountSelected;
        }

        public override void Unbind()
        {
            _window.AmountSelected -= OnAmountSelected;
        }

        private void OnAmountSelected(int amount)
        {
            Arguments.OnAmountPickedCallback?.Invoke(amount);
        }
    }

    public readonly struct AmountPickerWindowControllerArguments : IWindowControllerArguments
    {
        public readonly int MinAmount;
        public readonly int MaxAmount;
        public readonly Action<int> OnAmountPickedCallback;

        public AmountPickerWindowControllerArguments(int minAmount, int maxAmount, Action<int> onAmountPicked)
        {
            MinAmount = minAmount;
            MaxAmount = maxAmount;
            OnAmountPickedCallback = onAmountPicked;
        }
    }
}
