using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    public sealed class StatusEffectTrait : FunctionalItemTraitBase
    {
        [field: SerializeField]
        public StatusEffectDefinition StatusEffect { get; private set; }

        public override bool CanTrigger(in ItemTriggerContext context)
        {
            if (!base.CanTrigger(context)) return false;

            // OnUse requires additional checks
            if (context.Trigger != ItemTrigger.OnUse) return true;
            if (context.Payload is not ItemUseSettings itemUseSettings) return true;
            if (itemUseSettings.OverrideStatusEffects) return true;

            // Don't trigger if the user already has the status effect.
            return context.User.TryGetModule<StatusEffectModule>(out var statusEffectModule)
                && !statusEffectModule.StatusEffects.GetStatusEffects().Any(se => se.Definition == StatusEffect);
        }
    }
}
