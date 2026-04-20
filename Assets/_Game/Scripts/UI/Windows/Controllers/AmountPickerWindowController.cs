using System;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class AmountPickerWindowController : WindowControllerBase<AmountPickerWindow, AmountPickerWindowControllerArguments>
    {
        private AmountPickerWindow _window;

        private Action<int> _onAmountPickedCallback;

        public override void Initialize(AmountPickerWindowControllerArguments arguments)
        {
            base.Initialize(arguments);

            _onAmountPickedCallback = arguments.OnAmountPickedCallback;
            _window.Render(arguments.MinAmount, arguments.MaxAmount);
        }

        public override void Bind(AmountPickerWindow window)
        {
            _window = window;

            _window.AmountSelected += OnAmountSelected;
        }

        public override void Dispose()
        {
            _window.AmountSelected -= OnAmountSelected;
        }

        private void OnAmountSelected(int amount)
        {
            _onAmountPickedCallback?.Invoke(amount);
        }
    }
}
