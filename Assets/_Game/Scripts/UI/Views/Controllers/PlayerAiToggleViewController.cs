using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class PlayerAiToggleViewController : ViewControllerBase<PlayerAiToggleView>
    {
        private readonly PlayerContext _playerContext;
        private PlayerAiToggleViewData _viewData;

        public PlayerAiToggleViewController(
            PlayerAiToggleView view,
            PlayerAiToggleViewData viewData,
            PlayerContext playerContext)
        {
            _playerContext = playerContext;

            Initialize(view);
            Bind(viewData);
        }

        public void Bind(PlayerAiToggleViewData viewData)
        {
            _viewData = viewData;
            _viewData.Changed -= OnViewDataChanged;
            _viewData.Changed += OnViewDataChanged;

            _viewData.SetEntityId(_playerContext.ObservablePlayerId);
        }

        public void Unbind()
        {
            _viewData.Changed -= OnViewDataChanged;
            _viewData = null;
        }

        private void OnViewDataChanged()
        {
            _playerContext.SetPlayerAiEnabled(_viewData.IsAIEnabled);
            Render();
        }

        protected override void OnRender()
        {
            if (_viewData != null)
            {
                View.RequestRender(_viewData);
            }

            if (View != null)
            {
                View.ValueChanged -= OnValueChanged;
                View.ValueChanged += OnValueChanged;
            }
        }

        public override void Dispose()
        {
            if (View != null) View.ValueChanged -= OnValueChanged;
            Unbind();
            base.Dispose();
            _viewData?.Dispose();
        }

        private void OnValueChanged(bool enabled)
        {
            _viewData.SetAIEnabled(enabled);
        }
    }
}
