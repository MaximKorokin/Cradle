using System;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class RestrictionStateModule : EntityModuleBase
    {
        private readonly int[] _counts;
        private RestrictionState _state;

        public RestrictionState State => _state;

        public RestrictionStateModule()
        {
            _counts = new int[32];
        }

        public bool Has(RestrictionState state)
        {
            return (_state & state) != 0;
        }

        public void Add(RestrictionState state)
        {
            int index = BitIndex(state);

            _counts[index]++;

            if (_counts[index] == 1)
                _state |= state;
        }

        public void Remove(RestrictionState state)
        {
            int index = BitIndex(state);

            if (_counts[index] == 0)
                return;

            _counts[index]--;

            if (_counts[index] == 0)
                _state &= ~state;
        }

        private static int BitIndex(RestrictionState state)
        {
            int value = (int)state;
            int index = 0;

            while ((value >>= 1) != 0)
                index++;

            return index;
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
