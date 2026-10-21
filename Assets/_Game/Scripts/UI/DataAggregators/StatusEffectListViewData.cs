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
            _statusEffectModule.StatusEffects.StatusEffectChanged -= OnStatusEffectChanged;
            _statusEffectModule.StatusEffects.StatusEffectChanged += OnStatusEffectChanged;
        }

        private void OnStatusEffectChanged(StatusEffectChange statusEffectChange)
        {
            // flex :D
            if (statusEffectChange.Kind is StatusEffectChangeKind.Added or StatusEffectChangeKind.Removed)
                NotifyChanged();
        }

        public override void Dispose()
        {
            _statusEffectModule.StatusEffects.StatusEffectChanged -= OnStatusEffectChanged;
            base.Dispose();
        }

        public IEnumerable<StatusEffectSnapshot> GetStatusEffects(StatusEffectCategory statusEffectCategory = StatusEffectCategory.None)
        {
            return statusEffectCategory switch
            {
                StatusEffectCategory.Buff => Buffs,
                StatusEffectCategory.Debuff => Debuffs,
                _ => Buffs.Concat(Debuffs)
            };
        }
    }
}
