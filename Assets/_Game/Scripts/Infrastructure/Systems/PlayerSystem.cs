using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public class PlayerSystem : SystemBase
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly PlayerContext _playerContext;

        public PlayerSystem(PlayerContext playerContext, IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;
            _playerContext = playerContext;
            _globalEventBus.Subscribe<EntityDiedEvent>(OnEntityDiedEvent);
        }

        public override void Dispose()
        {
            base.Dispose();
            _globalEventBus.Unsubscribe<EntityDiedEvent>(OnEntityDiedEvent);
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
