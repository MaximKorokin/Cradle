using Assets._Game.Scripts.UI.DataAggregators;
using System;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class CompactPlayerStateViewController : IDisposable
    {
        private readonly CompactPlayerStateView _compactPlayerStateView;
        private readonly PlayerStateViewData _playerStateViewData;

        public CompactPlayerStateViewController(
            CompactPlayerStateView compactPlayerStateView,
            PlayerStateViewData playerStateViewData)
        {
            _compactPlayerStateView = compactPlayerStateView;
            _playerStateViewData = playerStateViewData;
        }

        public void Render()
        {
            _compactPlayerStateView.Redraw(_playerStateViewData);
        }

        public void Dispose()
        {
            _compactPlayerStateView.Clear();
        }
    }
}
