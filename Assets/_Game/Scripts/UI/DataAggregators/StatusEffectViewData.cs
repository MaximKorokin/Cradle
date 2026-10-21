using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class StatusEffectViewData : EntityBoundDataAggregatorBase
    {
        private StatusEffectModule _statusEffectModule;
        private StatusEffectDefinition _statusEffectDefinition;

        public StatusEffectSnapshot StatusEffectSnapshot =>
            _statusEffectModule.StatusEffects.HasStatusEffect(_statusEffectDefinition)
                ? _statusEffectModule.StatusEffects.GetStatusEffect(_statusEffectDefinition).Snapshot
                : default;

        public StatusEffectViewData(EntityRepository entityRepository) : base(entityRepository)
        {
        }

        public void SetStatusEffect(StatusEffectDefinition statusEffectDefinition)
        {
            _statusEffectDefinition = statusEffectDefinition;
        }

        protected override void OnBoundEntityChanged()
        {
            Entity.TryGetModule(out _statusEffectModule);
            if (_statusEffectModule != null)
                _statusEffectModule.StatusEffects.StatusEffectChanged += OnStatusEffectChanged;
        }

        public override void Dispose()
        {
            if (_statusEffectModule != null)
                _statusEffectModule.StatusEffects.StatusEffectChanged -= OnStatusEffectChanged;

            base.Dispose();
        }

        private void OnStatusEffectChanged(StatusEffectChange statusEffectChange)
        {
            if (statusEffectChange.StatusEffect.Definition == _statusEffectDefinition)
                NotifyChanged();
        }
    }
}
