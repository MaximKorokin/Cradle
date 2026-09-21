using System;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ConfirmationWindowController : WindowControllerBase<ConfirmationWindow, ConfirmationWindowControllerArguments>
    {
        protected override void OnBind()
        {
            base.OnBind();

            Window.Render(Arguments.Title, Arguments.Message);
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
