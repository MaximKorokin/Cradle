using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.UI.Systems;
using System;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public class PlayerRevivePromptSystem : EntitySystemBase
    {
        private readonly IPlayerProvider _playerProvider;

        protected override EntityQuery EntityQuery { get; } = new(RestrictionState.Disabled);

        public PlayerRevivePromptSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository repository,
            IPlayerProvider playerProvider) : base(globalEventBus, repository)
        {
            _playerProvider = playerProvider;

            TrackEntityEvent<RestrictionStateChangedEvent>(OnRestrictionStateChanged);
        }

        private void OnRestrictionStateChanged(Entity entity, RestrictionStateChangedEvent e)
        {
            if (e.State == RestrictionState.Dead && entity == _playerProvider.Player)
            {
                if (e.Kind == RestrictionStateChangeKind.Added)
                {
                    GlobalEventBus.Publish(PlayerPromptRequest.GetShowRequest("You Died", "Revive", OnReviveButtonClicked));
                }
                else if (e.Kind == RestrictionStateChangeKind.Removed)
                {
                    GlobalEventBus.Publish(PlayerPromptRequest.GetHideRequest());
                }
            }
        }

        private void OnReviveButtonClicked()
        {
            GlobalEventBus.Publish(new EntityReviveRequest(_playerProvider.Player, false));
        }
    }
}
