using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public class RewardSystem : SystemBase
    {
        public RewardSystem(IGlobalEventBus globalEventBus) : base(globalEventBus)
        {
            TrackGlobalEvent<EntityDiedEvent>(OnEntityDied);
        }

        private void OnEntityDied(EntityDiedEvent e)
        {
            if (e.Victim.TryGetModule(out RestrictionStateModule restrictionStateModule) && restrictionStateModule.Has(RestrictionState.Disabled))
                return;

            if (!e.Victim.TryGetModule(out RewardModule rewardModule))
                return;

            GlobalEventBus.Publish(new AddExperienceRequestEvent(e.Victim, e.Killer, rewardModule.Experience));

            if (!e.Victim.TryGetModule(out SpatialModule spatial))
                return;

            GlobalEventBus.Publish(new LootDropRequestedEvent(spatial.Position, rewardModule.LootTable));
        }
    }

    public readonly struct AddExperienceRequestEvent : IGlobalEvent
    {
        public Entity Source { get; }
        public Entity Target { get; }
        public long Experience { get; }

        public AddExperienceRequestEvent(Entity source, Entity target, long experience)
        {
            Source = source;
            Target = target;
            Experience = experience;
        }
    }
}
