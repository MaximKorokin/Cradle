using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Shared.Extensions;
using System;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public class PlayerSystem : EntitySystemBase
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly PlayerContext _playerContext;

        protected override EntityQuery EntityQuery => throw new NotImplementedException();

        public PlayerSystem(EntityRepository repository, DispatcherService dispatcher, PlayerContext playerContext, IGlobalEventBus globalEventBus) : base(repository, dispatcher)
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

        protected override bool Filter(Entity entity)
        {
            return true;
        }
    }
}
