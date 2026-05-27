using Assets._Game.Scripts.Infrastructure.Game;
using System;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class PlayerAiToggleViewController : IDisposable
    {
        private readonly PlayerAiToggleView _view;
        private readonly PlayerContext _playerContext;

        public PlayerAiToggleViewController(PlayerAiToggleView view, PlayerContext playerContext)
        {
            _view = view;
            _playerContext = playerContext;
        }

        public void Render()
        {
            _view.ValueChanged += OnValueChanged;
        }

        public void Dispose()
        {
            _view.ValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(bool enabled)
        {
            _playerContext.SetPlayerAiEnabled(enabled);
        }
    }
}
