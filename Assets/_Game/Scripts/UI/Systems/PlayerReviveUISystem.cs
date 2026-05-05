using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class PlayerReviveUISystem : UISystemBase
    {
        private IPlayerProvider _playerProvider;

        [Inject]
        private void Construct(
            IGlobalEventBus globalEventBus,
            IPlayerProvider playerProvider)
        {
            BaseConstruct(globalEventBus);

            _playerProvider = playerProvider;

            TrackGlobalEvent<EntityDiedEvent>(OnEntityDied);
            TrackGlobalEvent<EntityRevivedEvent>(OnEntityRevived);
        }

        private void OnReviveButtonClicked()
        {
            GlobalEventBus.Publish(new EntityReviveRequest(_playerProvider.Player, false));
        }

        private void OnEntityDied(EntityDiedEvent e)
        {
            if (e.Victim == _playerProvider.Player)
            {
                GlobalEventBus.Publish(InteractionPromptViewRequest.ShowRequest("You Died", "Revive", OnReviveButtonClicked));
            }
        }

        private void OnEntityRevived(EntityRevivedEvent e)
        {
            if (e.Entity == _playerProvider.Player)
            {
                GlobalEventBus.Publish(InteractionPromptViewRequest.HideRequest());
            }
        }
    }
}
