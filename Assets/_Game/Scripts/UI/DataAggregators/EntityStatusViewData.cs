using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using static UnityEngine.EventSystems.EventTrigger;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class EntityStatusViewData : EntityBoundDataAggregatorBase
    {
        private Entity _currentEntity;

        public float CurrentHp => Entity.GetModule<HealthModule>().CurrentHealth;
        public float MaxHp => Entity.GetModule<HealthModule>().MaxHealth;
        public float Level => Entity.GetModule<LevelingModule>().Level;
        public float NormalizedExperience => Entity.GetModule<LevelingModule>().GetNormalizedExperience();

        public EntityStatusViewData(EntityRepository entityRepository) : base(entityRepository) { }

        protected override void OnBoundEntityChanged()
        {
            UnsubscribeFromPlayerModules(_currentEntity);
            _currentEntity = Entity;
            SubscribeToPlayerModules(_currentEntity);
        }

        private void SubscribeToPlayerModules(Entity entity)
        {
            if (entity == null) return;

            entity.GetModule<StatModule>().Stats.StatChanged += OnStatChanged;
            entity.GetModule<HealthModule>().Changed += OnHealthChanged;
            entity.GetModule<LevelingModule>().Changed += OnLevelingModuleChanged;
        }

        private void UnsubscribeFromPlayerModules(Entity entity)
        {
            if (entity == null) return;

            entity.GetModule<StatModule>().Stats.StatChanged -= OnStatChanged;
            entity.GetModule<HealthModule>().Changed -= OnHealthChanged;
            entity.GetModule<LevelingModule>().Changed -= OnLevelingModuleChanged;
        }

        private void OnStatChanged(StatId statId)
        {
            if (statId == StatId.HpMax)
            {
                NotifyChanged();
            }
        }

        private void OnHealthChanged(float previous, float current)
        {
            NotifyChanged();
        }

        private void OnLevelingModuleChanged()
        {
            NotifyChanged();
        }

        public override void Dispose()
        {
            base.Dispose();

            UnsubscribeFromPlayerModules(_currentEntity);
        }
    }
}
