using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Infrastructure.Game;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public class RewardSystem : SystemBase
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly PlayerContext _playerContext;

        public RewardSystem(IGlobalEventBus globalEventBus, PlayerContext playerContext)
        {
            _globalEventBus = globalEventBus;
            _playerContext = playerContext;

            _globalEventBus.Subscribe<EntityDiedEvent>(OnEntityDied);
        }

        public override void Dispose()
        {
            base.Dispose();
            _globalEventBus.Unsubscribe<EntityDiedEvent>(OnEntityDied);
        }

        private void OnEntityDied(EntityDiedEvent e)
        {
            if (!e.Victim.TryGetModule(out RewardModule rewardModule))
                return;

            GiveExperience(e.Killer, rewardModule.Experience);

            if (!e.Victim.TryGetModule(out SpatialModule spatial))
                return;

            _globalEventBus.Publish(new LootDropRequestedEvent(spatial.Position, rewardModule.LootTable));
        }

        private void GiveExperience(Entity killer, int experienve)
        {
            if (killer == null)
                return;

            if (killer != _playerContext.Player)
                return;

            if (!killer.TryGetModule(out StatModule statModule))
                return;

            statModule.AddBase(StatId.Experience, experienve);
        }
    }
}
