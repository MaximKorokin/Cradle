using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class StatsViewData : EntityBoundDataAggregatorBase
    {
        private readonly EntityRepository _entityRepository;
        private StatModule _statModule;

        public IEnumerable<(string, string)> Stats { get; private set; }
        public event Action Changed;

        public StatsViewData(EntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        protected override void OnBoundEntityChanged(string entityId)
        {
            if (_statModule != null)
            {
                _statModule.Stats.Changed -= OnStatsChanged;
            }

            var entity = string.IsNullOrEmpty(entityId) ? null : _entityRepository.Get(entityId);
            _statModule = entity?.GetModule<StatModule>();

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

            Changed?.Invoke();
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
