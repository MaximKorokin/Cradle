using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Systems;
using System;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class AmountPickerWindowController : WindowControllerBase<AmountPickerWindow, AmountPickerWindowControllerArguments>
    {
        private readonly IGlobalEventBus _globalEventBus;

        public AmountPickerWindowController(
            IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;
        }

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
            _globalEventBus.Publish(new WindowCloseRequest(Window));
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
