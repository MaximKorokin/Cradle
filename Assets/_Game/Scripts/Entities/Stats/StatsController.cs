using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Stats
{
    public interface IStatsReadOnly
    {
        event Action<StatId> StatChanged;
        event Action Changed;

        IEnumerable<(StatId Id, float Base, float Final)> Enumerate();
        bool Has(StatId id);
        float Get(StatId id);
        float GetBase(StatId id);
    }

    public class StatsController : IStatsReadOnly
    {
        private readonly Dictionary<StatId, Stat> _stats = new();
        private readonly StatRegulator _statRegulator;

        public event Action<StatId> StatChanged;
        public event Action Changed;

        public StatsController(IEnumerable<(StatId Id, float BaseValue)> initial, StatRegulator statRegulator)
        {
            foreach (var (id, baseValue) in initial)
                _stats[id] = new Stat(baseValue);

            _statRegulator = statRegulator;
        }

        public IEnumerable<(StatId Id, float Base, float Final)> Enumerate()
        {
            foreach (var (id, stat) in _stats)
                yield return (id, stat.BaseValue, stat.Calculate());
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

            _statRegulator.RegulateBeforeChange(this, id, ref value);

            s.SetBase(value);

            _statRegulator.RegulateAfterChange(this, id);

            RaiseChanged(id);
        }

        public void AddModifiers(object source, IEnumerable<StatModifier> modifiers)
        {
            foreach (var m in modifiers)
            {
                _statRegulator.RegulateModifier(this, m.Stat, m);

                var stat = GetOrCreate(m.Stat);
                stat.AddModifier(source, m);

                _statRegulator.RegulateAfterChange(this, m.Stat);

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
                    _statRegulator.RegulateAfterChange(this, kv.Key);
                    RaiseChanged(kv.Key);
                }
            }
        }

        private Stat GetOrCreate(StatId id)
        {
            if (_stats.TryGetValue(id, out var s)) return s;
            s = new Stat(0);
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
