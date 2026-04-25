using Assets._Game.Scripts.Entities.Interactions;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Shared.Extensions;
using UnityEngine;
using static Assets._Game.Scripts.Entities.Interactions.InteractionDefinition;

namespace Assets._Game.Scripts.Items.Traits
{
    public sealed class InteractionTrait : FunctionalItemTraitBase
    {
        [field: SerializeField]
        public InteractionDefinition Interaction { get; private set; }

        public override bool CanTrigger(in ItemTriggerContext context)
        {
            if (!base.CanTrigger(context)) return false;
            if (context.Trigger != ItemTrigger.OnUse) return true;
            if (context.Payload is not ItemUseSettings itemUseSettings) return true;

            // If the interaction doesn't have a heal step, then the trait can trigger without any condition.
            if (!Interaction.Root.TryGetStep<HealData>(out var _)) return true;

            // If the item is being used with HP% condition, check if the user's HP% meets the condition.
            return context.User.TryGetModule<HealthModule>(out var healthModule)
                && healthModule.HealthRatio < (itemUseSettings.HpPercent / 100f);
        }
    }
}
