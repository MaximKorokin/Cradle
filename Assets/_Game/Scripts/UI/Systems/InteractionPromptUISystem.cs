using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Views;
using System;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class InteractionPromptUISystem : UISystemBase
    {
        private InteractionPromptView _interactionPromptView;

        [Inject]
        private void Construct(
            IGlobalEventBus globalEventBus,
            InteractionPromptView locationTransitionView)
        {
            BaseConstruct(globalEventBus);

            _interactionPromptView = locationTransitionView;

            TrackGlobalEvent<InteractionPromptViewRequest>(OnInteractionPromptViewRequested);
        }

        private void OnInteractionPromptViewRequested(InteractionPromptViewRequest request)
        {
            if (request.Show)
            {
                _interactionPromptView.Show(request.PromptText, request.ButtonText, request.Callback);
            }
            else
            {
                _interactionPromptView.Hide();
            }
        }
    }

    public readonly struct InteractionPromptViewRequest : IGlobalEvent
    {
        public string PromptText { get; }
        public string ButtonText { get; }
        public bool Show { get; }
        public Action Callback { get; }

        private InteractionPromptViewRequest(string promptText, string buttonText, bool show, Action callback = null)
        {
            PromptText = promptText;
            ButtonText = buttonText;
            Show = show;
            Callback = callback;
        }

        public static InteractionPromptViewRequest ShowRequest(string promptText, string buttonText, Action callback = null)
        {
            return new InteractionPromptViewRequest(promptText, buttonText, true, callback);
        }

        public static InteractionPromptViewRequest HideRequest()
        {
            return new InteractionPromptViewRequest(string.Empty, string.Empty, false, null);
        }
    }
}
