using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public class RewardSystem : SystemBase
    {
        private readonly IGlobalEventBus _globalEventBus;

        public RewardSystem(IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;

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

            _globalEventBus.Publish(new AddExperienceRequestEvent(e.Killer, rewardModule.Experience));

            if (!e.Victim.TryGetModule(out SpatialModule spatial))
                return;

            _globalEventBus.Publish(new LootDropRequestedEvent(spatial.Position, rewardModule.LootTable));
        }
    }

    public readonly struct AddExperienceRequestEvent : IGlobalEvent
    {
        public Entity Target { get; }
        public long Experience { get; }

        public AddExperienceRequestEvent(Entity target, long experience)
        {
            Target = target;
            Experience = experience;
        }
    }
}
