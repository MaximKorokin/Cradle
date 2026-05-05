using System;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class AmountPickerWindowController : WindowControllerBase<AmountPickerWindow, AmountPickerWindowControllerArguments>
    {
        private AmountPickerWindow _window;

        private Action<int> _onAmountPickedCallback;
        private int _minAmount;
        private int _maxAmount;

        public override void Initialize(AmountPickerWindowControllerArguments arguments)
        {
            base.Initialize(arguments);

            _onAmountPickedCallback = arguments.OnAmountPickedCallback;
            _minAmount = arguments.MinAmount;
            _maxAmount = arguments.MaxAmount;
        }

        public override void Bind(AmountPickerWindow window)
        {
            _window = window;
            _window.Render(_minAmount, _maxAmount);
            _window.AmountSelected += OnAmountSelected;
        }

        public override void Unbind()
        {
            _window.AmountSelected -= OnAmountSelected;
        }

        private void OnAmountSelected(int amount)
        {
            _onAmountPickedCallback?.Invoke(amount);
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
