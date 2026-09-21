using System;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class AmountPickerWindowController : WindowControllerBase<AmountPickerWindow, AmountPickerWindowControllerArguments>
    {
        protected override void OnBind()
        {
            base.OnBind();

            Window.AmountSelected += OnAmountSelected;
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            Window.AmountSelected -= OnAmountSelected;
        }

        private void OnAmountSelected(int amount)
        {
            Arguments.OnAmountPickedCallback?.Invoke(amount);
        }

        protected override void Redraw()
        {
            Window.Render(Arguments.MinAmount, Arguments.MaxAmount);
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
