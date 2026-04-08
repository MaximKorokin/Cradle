using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using System;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public class EntityNameplateViewData : DataAggregatorBase
    {
        private readonly Entity _entity;

        public string Name { get; private set; }
        public float HealthPercent { get; private set; }

        public event Action Changed;

        public EntityNameplateViewData(Entity entity)
        {
            _entity = entity;

            Name = _entity.Definition.VariantName;
            HealthPercent = GetHealthPercent(_entity);

            if (_entity.TryGetModule<StatModule>(out var statModule))
            {
                statModule.Stats.StatChanged += OnEntityStatChanged;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_entity.TryGetModule<StatModule>(out var statModule))
            {
                statModule.Stats.StatChanged -= OnEntityStatChanged;
            }
        }

        private void OnEntityStatChanged(StatId statId)
        {
            if (statId != StatId.HpCurrent && statId != StatId.HpMax) return;

            HealthPercent = GetHealthPercent(_entity);

            Changed?.Invoke();
        }

        private static float GetHealthPercent(Entity entity)
        {
            if (!entity.TryGetModule<StatModule>(out var statModule)) return 1;

            var currentHp = statModule.Stats.Get(StatId.HpCurrent);
            var maxHp = statModule.Stats.Get(StatId.HpMax);
            
            if (maxHp <= 0) return 0;

            return currentHp / maxHp;
        }
    }
}
