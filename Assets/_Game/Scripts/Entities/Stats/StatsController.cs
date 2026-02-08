using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Stats
{
    public interface IStatsReadOnly
    {
        float Get(StatId id);
        float GetBase(StatId id);
    }

    public class StatsController : IStatsReadOnly
    {
        private readonly Dictionary<StatId, EntityStat> _stats = new();

        public event Action<StatId> StatChanged;
        public event Action Changed;

        public StatsController(IEnumerable<(StatId Id, float BaseValue)> initial)
        {
            foreach (var (id, baseValue) in initial)
                _stats[id] = new EntityStat(baseValue);
        }

        public bool Has(StatId id) => _stats.ContainsKey(id);

        public float Get(StatId id)
        {
            if (!_stats.TryGetValue(id, out var s))
                throw new KeyNotFoundException($"Stat not found: {id}");
            return s.Calculate();
        }

        public float GetBase(StatId id)
        {
            if (!_stats.TryGetValue(id, out var s))
                throw new KeyNotFoundException($"Stat not found: {id}");
            return s.BaseValue;
        }

        public void SetBase(StatId id, float value)
        {
            var s = GetOrCreate(id);
            if (Math.Abs(s.BaseValue - value) < 0.0001f) return;

            s.SetBase(value);
            RaiseChanged(id);
        }

        public void AddModifiers(object source, IEnumerable<StatModifier> modifiers)
        {
            foreach (var m in modifiers)
            {
                var stat = GetOrCreate(m.Stat);
                stat.AddModifier(source, m);
                RaiseChanged(m.Stat);
            }
        }

        public void RemoveModifiers(object source)
        {
            foreach (var kv in _stats)
            {
                int removed = kv.Value.RemoveBySource(source);
                if (removed > 0)
                {
                    RaiseChanged(kv.Key);
                }
            }
        }

        private EntityStat GetOrCreate(StatId id)
        {
            if (_stats.TryGetValue(id, out var s)) return s;
            s = new EntityStat(0);
            _stats[id] = s;
            return s;
        }

        private void RaiseChanged(StatId id)
        {
            StatChanged.Invoke(id);
            Changed.Invoke();
        }
    }
}
