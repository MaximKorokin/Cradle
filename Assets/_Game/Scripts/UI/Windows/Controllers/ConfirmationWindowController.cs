using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Systems;
using System;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ConfirmationWindowController : WindowControllerBase<ConfirmationWindow, ConfirmationWindowControllerArguments>
    {
        private readonly IGlobalEventBus _globalEventBus;

        public ConfirmationWindowController(
            IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;
        }

        protected override void OnBind()
        {
            base.OnBind();

            Window.ConfirmationResult += OnConfirmationResult;
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            Window.ConfirmationResult -= OnConfirmationResult;
        }

        private void OnConfirmationResult(bool confirmed)
        {
            Arguments.OnDecisionCallback?.Invoke(confirmed);
            _globalEventBus.Publish(new WindowCloseRequest(Window));
        }

        protected override void Redraw()
        {
            Window.Render(Arguments.Title, Arguments.Message);
        }
    }

    public readonly struct ConfirmationWindowControllerArguments : IWindowControllerArguments
    {
        public readonly string Title;
        public readonly string Message;
        public readonly Action<bool> OnDecisionCallback;

        public ConfirmationWindowControllerArguments(string title, string message, Action<bool> onDecisionCallback)
        {
            Title = title;
            Message = message;
            OnDecisionCallback = onDecisionCallback;
        }
    }
}
