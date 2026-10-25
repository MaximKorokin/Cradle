using Assets._Game.Scripts.Entities.Stats;
using System;
using Assets._Game.Scripts.Infrastructure.Persistence;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class HealthModule : EntityModuleBase
    {
        private StatModule _statModule;
        private float _maxHealth;
        private float _currentHealth;

        public float MaxHealth => _maxHealth;

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

            if (!Entity.TryGetModule<StatModule>(out _statModule))
            {
                SLog.Error($"No {typeof(StatModule)} found on entity {Entity} but {typeof(HealthModule)} requires it");
                return;
            }

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

        public void SetCurrentHealth(float value)
        {
            CurrentHealth = value;
        }

    }

    public sealed class HealthModuleFactory : IEntityModuleFactory, IEntityModulePersistance
    {
        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModuleDefinition<StatsModuleDefinition>(out _)) return null;

            return new HealthModule();
        }

        public void Apply(Entity entity, EntitySave entitySave)
        {
            if (entitySave.HealthSave == null) return;
            if (!entity.TryGetModule<HealthModule>(out var healthModule)) return;
            if (!entity.TryGetModule<RestrictionStateModule>(out var restrictionStateModule)) return;

            var currentHealth = entitySave.HealthSave.CurrentHealth;
            if (entity.IsCreated)
            {
                ApplyHealth(entitySave.HealthSave, healthModule, restrictionStateModule);
            }
            else
            {
                entity.SubscribeOnce<EntityViewBoundEvent>(
                    _ => ApplyHealth(entitySave.HealthSave, healthModule, restrictionStateModule),
                    int.MinValue);
            }
        }

        private void ApplyHealth(HealthSave healthSave, HealthModule healthModule, RestrictionStateModule restrictionStateModule)
        {
            healthModule.SetCurrentHealth(healthSave.CurrentHealth);
            if (healthSave.IsDead)
            {
                restrictionStateModule.Add(RestrictionState.Dead);
            }
        }

        public void Save(Entity entity, EntitySave entitySave)
        {
            if (!entity.TryGetModule<HealthModule>(out var healthModule)) return;
            if (!entity.TryGetModule<RestrictionStateModule>(out var restrictionStateModule)) return;

            entitySave.HealthSave = new HealthSave
            {
                CurrentHealth = healthModule.CurrentHealth,
                IsDead = restrictionStateModule.Has(RestrictionState.Dead)
            };
        }
    }
}
