using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.StatusEffects;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class EntityStatusViewData : EntityBoundDataAggregatorBase
    {
        public EntityStatusViewData(EntityRepository entityRepository) : base(entityRepository) { }

        protected override void OnBoundEntityChanged(string entityId)
        {
            UnsubscribeFromPlayerModules();
            SubscribeToPlayerModules();
        }

        private void SubscribeToPlayerModules()
        {
            Entity.GetModule<StatModule>().Stats.StatChanged += OnStatChanged;
            Entity.GetModule<HealthModule>().Changed += OnHealthChanged;
            Entity.GetModule<StatusEffectModule>().StatusEffects.Changed += OnStatusEffectsControllerChanged;
            Entity.GetModule<LevelingModule>().Changed += OnLevelingModuleChanged;
        }

        private void UnsubscribeFromPlayerModules()
        {
            Entity.GetModule<StatModule>().Stats.StatChanged -= OnStatChanged;
            Entity.GetModule<HealthModule>().Changed -= OnHealthChanged;
            Entity.GetModule<StatusEffectModule>().StatusEffects.Changed -= OnStatusEffectsControllerChanged;
            Entity.GetModule<LevelingModule>().Changed -= OnLevelingModuleChanged;
        }

        public float CurrentHp => Entity.GetModule<HealthModule>().CurrentHealth;
        public float MaxHp => Entity.GetModule<HealthModule>().MaxHealth;
        public float Level => Entity.GetModule<LevelingModule>().Level;
        public float NormalizedExperience => Entity.GetModule<LevelingModule>().GetNormalizedExperience();

        public IEnumerable<StatusEffectSnapshot> Buffs =>
            Entity.GetModule<StatusEffectModule>().StatusEffects.GetStatusEffectsForCategory(StatusEffectCategory.Buff).Select(s => s.Snapshot);

        public IEnumerable<StatusEffectSnapshot> Debuffs =>
            Entity.GetModule<StatusEffectModule>().StatusEffects.GetStatusEffectsForCategory(StatusEffectCategory.Debuff).Select(s => s.Snapshot);

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

        private void OnStatusEffectsControllerChanged()
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

            UnsubscribeFromPlayerModules();
        }
    }
}
