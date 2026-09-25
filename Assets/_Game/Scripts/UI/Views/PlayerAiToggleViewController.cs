using Assets._Game.Scripts.Infrastructure.Game;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class PlayerAiToggleViewController : ViewControllerBase<PlayerAiToggleView>
    {
        private readonly PlayerContext _playerContext;

        public PlayerAiToggleViewController(PlayerAiToggleView view, PlayerContext playerContext)
        {
            Initialize(view);
            _playerContext = playerContext;
        }

        protected override void OnRender()
        {
            View.ValueChanged -= OnValueChanged;
            View.ValueChanged += OnValueChanged;
        }

        public override void Dispose()
        {
            if (View != null) View.ValueChanged -= OnValueChanged;
            base.Dispose();
        }

        private void OnValueChanged(bool enabled)
        {
            _playerContext.SetPlayerAiEnabled(enabled);
        }
    }
}
