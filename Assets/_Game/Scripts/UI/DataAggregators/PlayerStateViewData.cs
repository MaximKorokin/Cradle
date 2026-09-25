using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.StatusEffects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class PlayerStateViewData : EntityBoundDataAggregatorBase
    {
        public event Action Changed;

        public PlayerStateViewData(EntityRepository entityRepository) : base(entityRepository) { }

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
                Changed?.Invoke();
            }
        }

        private void OnHealthChanged(float previous, float current)
        {
            Changed?.Invoke();
        }

        private void OnStatusEffectsControllerChanged()
        {
            Changed?.Invoke();
        }

        private void OnLevelingModuleChanged()
        {
            Changed?.Invoke();
        }

        public override void Dispose()
        {
            base.Dispose();

            UnsubscribeFromPlayerModules();
        }
    }
}
