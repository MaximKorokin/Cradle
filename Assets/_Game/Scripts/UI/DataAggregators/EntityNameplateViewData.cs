using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using System;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public class EntityNameplateViewData : DataAggregatorBase
    {
        private readonly HealthModule _healthModule;

        public string Level { get; private set; }
        public string Name { get; private set; }
        public float HealthRatio { get; private set; }

        public bool ShouldViewHealthBar => _healthModule != null;

        public event Action Changed;

        public EntityNameplateViewData(Entity entity)
        {
            if (entity.TryGetModule<HealthModule>(out _healthModule))
            {
                HealthRatio = _healthModule.HealthRatio;
                _healthModule.Changed += OnHealthChanged;
            }

            Level = entity.TryGetModule<LevelingModule>(out var levelingModule) ? levelingModule.Level.ToString() : "";
            Name = entity.Definition.DisplayName;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_healthModule != null)
            {
                _healthModule.Changed -= OnHealthChanged;
            }
        }

        private void OnHealthChanged(float previous, float current)
        {
            if (_healthModule != null)
            {
                HealthRatio = _healthModule.HealthRatio;
            }

            Changed?.Invoke();
        }
    }
}
