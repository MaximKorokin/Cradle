using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.UI.Views;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class PlayerReviveUISystem : UISystemBase
    {
        private PlayerReviveView _playerReviveView;
        private IPlayerProvider _playerProvider;

        [Inject]
        private void Construct(
            IGlobalEventBus globalEventBus,
            PlayerReviveView playerReviveView,
            IPlayerProvider playerProvider)
        {
            BaseConstruct(globalEventBus);

            _playerReviveView = playerReviveView;
            _playerProvider = playerProvider;

            _playerReviveView.ReviveButtonClicked += OnReviveButtonClicked;

            TrackGlobalEvent<EntityDiedEvent>(OnEntityDied);
        }

        private void OnEntityDied(EntityDiedEvent e)
        {
            if (e.Victim == _playerProvider.Player)
            {
                _playerReviveView.Show();
            }
        }

        private void OnReviveButtonClicked()
        {
            GlobalEventBus.Publish(new EntityReviveRequest(_playerProvider.Player));
            _playerReviveView.Hide();
        }
    }
}
