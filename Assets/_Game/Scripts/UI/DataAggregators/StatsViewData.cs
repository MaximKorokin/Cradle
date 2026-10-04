using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public interface IStatsViewData : IEntityBoundDataAggregatorBase
    {
        IEnumerable<(string Name, string Value)> Stats { get; }
    }

    public sealed class StatsViewData : EntityBoundDataAggregatorBase, IStatsViewData
    {
        private StatModule _statModule;

        public IEnumerable<(string Name, string Value)> Stats { get; private set; }

        public StatsViewData(EntityRepository entityRepository) : base(entityRepository) { }

        protected override void OnBoundEntityChanged(string entityId)
        {
            if (_statModule != null)
            {
                _statModule.Stats.Changed -= OnStatsChanged;
            }

            _statModule = Entity?.GetModule<StatModule>();

            if (_statModule != null)
            {
                _statModule.Stats.Changed += OnStatsChanged;
            }

            UpdateData();
        }

        private void OnStatsChanged()
        {
            UpdateData();
        }

        private void UpdateData()
        {
            Stats = _statModule == null
                ? Array.Empty<(string, string)>()
                : _statModule.Stats.Enumerate()
                    .Select(stat => (stat.Id.ToString(), stat.Final.ToString()));

            NotifyChanged();
        }

        public override void Dispose()
        {
            if (_statModule != null)
            {
                _statModule.Stats.Changed -= OnStatsChanged;
            }

            base.Dispose();
        }
    }
}
