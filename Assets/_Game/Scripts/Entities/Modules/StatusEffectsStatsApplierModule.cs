using Assets._Game.Scripts.Entities.StatusEffects;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class StatusEffectsStatsApplierModule : EntityModuleBase
    {
        private readonly StatModule _statsModule;

        public StatusEffectsStatsApplierModule(StatModule statsModule)
        {
            _statsModule = statsModule;
        }

        protected override void OnAttach()
        {
            base.OnAttach();

            Subscribe<StatusEffectChangedEvent>(OnStatusEffectChanged);
        }

        private void OnStatusEffectChanged(StatusEffectChangedEvent e)
        {
            if (_statsModule == null) return;
            foreach (var modifier in e.StatusEffect.Definition.StatModifiers)
            {
                if (e.Kind == StatusEffectChangeKind.Added)
                {
                    _statsModule.AddModifiers(e.StatusEffect, new[] { modifier });
                }
                else if (e.Kind == StatusEffectChangeKind.Removed)
                {
                    _statsModule.RemoveModifiers(e.StatusEffect);
                }
            }
        }
    }
}
