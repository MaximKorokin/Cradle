using System;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class RestrictionStateModule : EntityModuleBase
    {
        public RestrictionState State { get; private set; }

        public bool Has(RestrictionState state)
        {
            return (State & state) != 0;
        }

        public void Add(RestrictionState state)
        {
            State |= state;
            Entity.Publish(new RestrictionStateChangedEvent(Entity, State));
        }

        public void Remove(RestrictionState state)
        {
            State &= ~state;
            Entity.Publish(new RestrictionStateChangedEvent(Entity, State));
        }
    }

    [Flags]
    public enum RestrictionState
    {
        None = 0,
        Disabled = 1 << 0,
        Dead = 1 << 1,
        Stunned = 1 << 2,
        Feared = 1 << 3,
    }

    public readonly struct RestrictionStateChangedEvent : IEntityEvent
    {
        public Entity Entity { get; }
        public RestrictionState State { get; }

        public RestrictionStateChangedEvent(Entity entity, RestrictionState state)
        {
            Entity = entity;
            State = state;
        }
    }
}
