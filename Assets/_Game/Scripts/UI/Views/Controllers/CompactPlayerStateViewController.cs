using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.DataAggregators;
namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class CompactPlayerStateViewController : ViewControllerBase<CompactPlayerStateView>
    {
        private readonly PlayerStateViewData _playerStateViewData;

        public CompactPlayerStateViewController(
            CompactPlayerStateView compactPlayerStateView,
            PlayerStateViewData playerStateViewData,
            IPlayerProvider playerProvider)
        {
            Initialize(compactPlayerStateView);

            _playerStateViewData = playerStateViewData;
            _playerStateViewData.SetEntityId(playerProvider.ObservablePlayerId);
            _playerStateViewData.Changed += OnPlayerStateChanged;
        }

        protected override void OnRender()
        {
            View.RequestRender(_playerStateViewData);
        }

        public override void Dispose()
        {
            _playerStateViewData.Changed -= OnPlayerStateChanged;
            View.Clear();
            base.Dispose();
        }

        private void OnPlayerStateChanged()
        {
            Render();
        }
    }
}
