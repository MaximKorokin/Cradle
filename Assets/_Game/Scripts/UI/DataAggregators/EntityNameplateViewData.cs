using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public class EntityNameplateWidgetData : EntityBoundDataAggregatorBase
    {
        private HealthModule _healthModule;

        public string Level { get; private set; }
        public string Name { get; private set; }
        public float HealthRatio { get; private set; }

        public bool ShouldViewHealthBar => _healthModule != null;

        public EntityNameplateWidgetData(EntityRepository entityRepository) : base(entityRepository)
        {
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_healthModule != null)
            {
                _healthModule.Changed -= OnHealthChanged;
            }
        }

        protected override void OnBoundEntityChanged()
        {
            if (_healthModule != null)
            {
                _healthModule.Changed -= OnHealthChanged;
            }

            if (!Entity.TryGetModule<HealthModule>(out _healthModule)) return;

            HealthRatio = _healthModule.HealthRatio;
            _healthModule.Changed += OnHealthChanged;

            Level = Entity.TryGetModule<LevelingModule>(out var levelingModule) ? levelingModule.Level.ToString() : "";
            Name = Entity.Definition.DisplayName;
        }

        private void OnHealthChanged(float previous, float current)
        {
            if (_healthModule == null || HealthRatio == _healthModule.HealthRatio) return;

            HealthRatio = _healthModule.HealthRatio;
            NotifyChanged();
        }
    }
}
