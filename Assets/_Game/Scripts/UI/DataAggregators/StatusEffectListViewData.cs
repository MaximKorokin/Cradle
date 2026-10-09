using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class StatusEffectListViewData : EntityBoundDataAggregatorBase
    {
        private StatusEffectModule _statusEffectModule;

        public IEnumerable<StatusEffectSnapshot> Buffs =>
            Entity.GetModule<StatusEffectModule>().StatusEffects.GetStatusEffectsForCategory(StatusEffectCategory.Buff).Select(s => s.Snapshot);

        public IEnumerable<StatusEffectSnapshot> Debuffs =>
            Entity.GetModule<StatusEffectModule>().StatusEffects.GetStatusEffectsForCategory(StatusEffectCategory.Debuff).Select(s => s.Snapshot);

        public StatusEffectListViewData(EntityRepository entityRepository) : base(entityRepository)
        {
        }

        protected override void OnBoundEntityChanged()
        {
            _statusEffectModule = Entity.GetModule<StatusEffectModule>();
            _statusEffectModule.StatusEffects.Changed -= OnStatusEffectsModuleChanged;
            _statusEffectModule.StatusEffects.Changed += OnStatusEffectsModuleChanged;
        }

        private void OnStatusEffectsModuleChanged()
        {
            NotifyChanged();
        }

        public override void Dispose()
        {
            _statusEffectModule.StatusEffects.Changed -= OnStatusEffectsModuleChanged;
            base.Dispose();
        }
    }
}
