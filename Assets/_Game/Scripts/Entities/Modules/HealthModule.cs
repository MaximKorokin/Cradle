using Assets._Game.Scripts.Entities.Stats;
using System;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class HealthModule : EntityModuleBase
    {
        private StatModule _statModule;
        private float _maxHealth;

        private float _currentHealth;

        public float CurrentHealth
        {
            get => _currentHealth;
            private set
            {
                var clamped = Math.Clamp(value, 0f, _maxHealth);

                if (Math.Abs(clamped - _currentHealth) < 0.0001f)
                    return;

                var previous = _currentHealth;
                _currentHealth = clamped;

                Changed?.Invoke(previous, _currentHealth);
            }
        }

        public float HealthRatio
        {
            get
            {
                if (_maxHealth <= 0) return 0f;
                return (float)CurrentHealth / _maxHealth;
            }
        }

        public Action<float, float> Changed;

        public override void Initialize()
        {
            base.Initialize();

            if (!Entity.TryGetModule<StatModule>(out var statModule))
            {
                SLog.Error($"No {typeof(StatModule)} found on entity {Entity} but {typeof(HealthModule)} requires it");
                return;
            }
            _statModule = statModule;

            _statModule.Stats.StatChanged += OnStatChanged;
            OnStatChanged(StatId.HpMax);
            CurrentHealth = _maxHealth;
        }

        private void OnStatChanged(StatId statId)
        {
            if (statId == StatId.HpMax)
            {
                _maxHealth = _statModule.Stats.Get(StatId.HpMax);
                CurrentHealth = Math.Min(CurrentHealth, _maxHealth);
            }
        }

        public float ApplyDamage(float damage)
        {
            if (damage <= 0) return 0;

            var before = CurrentHealth;
            CurrentHealth -= damage;

            return before - CurrentHealth;
        }

        public float Heal(float value)
        {
            if (value <= 0) return 0;

            var before = CurrentHealth;
            CurrentHealth += value;

            return CurrentHealth - before;
        }
    }

    public sealed class HealthModuleFactory : IEntityModuleFactory
    {
        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModuleDefinition<StatsModuleDefinition>(out var statsModuleDefinition)) return null;

            return new HealthModule();
        }
    }
}
