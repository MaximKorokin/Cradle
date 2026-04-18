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

        public event Action Changed;

        public EntityNameplateViewData(Entity entity)
        {
            _healthModule = entity.GetModule<HealthModule>();

            Level = entity.TryGetModule<LevelingModule>(out var levelingModule) ? levelingModule.Level.ToString() : "";
            Name = entity.Definition.VariantName;
            HealthRatio = _healthModule.HealthRatio;

            _healthModule.Changed += OnHealthChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            _healthModule.Changed -= OnHealthChanged;
        }

        private void OnHealthChanged(float previous, float current)
        {
            HealthRatio = _healthModule.HealthRatio;

            Changed?.Invoke();
        }
    }
}
