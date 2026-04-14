using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public class PlayerSystem : SystemBase
    {
        private readonly PlayerContext _playerContext;

        public PlayerSystem(
            IGlobalEventBus globalEventBus,
            PlayerContext playerContext) : base(globalEventBus)
        {
            _playerContext = playerContext;

            TrackGlobalEvent<EntityDiedEvent>(OnEntityDiedEvent);
        }

        private void OnEntityDiedEvent(EntityDiedEvent e)
        {
            if (e.Victim == _playerContext.Player)
            {
                SLog.Log("Player died");
            }
        }
    }
}
