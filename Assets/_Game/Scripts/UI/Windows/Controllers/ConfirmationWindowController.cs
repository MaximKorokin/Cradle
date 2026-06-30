using System;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ConfirmationWindowController : WindowControllerBase<Assets._Game.Scripts.UI.Windows.ConfirmationWindow, ConfirmationWindowControllerArguments>
    {
        private Assets._Game.Scripts.UI.Windows.ConfirmationWindow _window;

        public override void Bind(Assets._Game.Scripts.UI.Windows.ConfirmationWindow window)
        {
            _window = window;
            _window.Render(Arguments.Title, Arguments.Message);
            _window.ConfirmationResult += OnConfirmationResult;
        }

        public override void Unbind()
        {
            _window.ConfirmationResult -= OnConfirmationResult;
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
