using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using System;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class FunctionalItemTraitFormatter : IDataFormatter<FunctionalItemTraitBase, string>
    {
        private readonly InteractionDefinitionFormatter _interactionDefinitionFormatter;
        private readonly StatModifiersFormatter _statModifiersFormatter;
        private readonly StatusEffectFormatter _statusEffectFormatter;
        private readonly ActionDefinitionFormatter _actionDefinitionFormatter;

        public FunctionalItemTraitFormatter(
            InteractionDefinitionFormatter interactionDefinitionFormatter,
            StatModifiersFormatter statModifiersFormatter,
            StatusEffectFormatter statusEffectFormatter,
            ActionDefinitionFormatter actionDefinitionFormatter)
        {
            _interactionDefinitionFormatter = interactionDefinitionFormatter;
            _statModifiersFormatter = statModifiersFormatter;
            _statusEffectFormatter = statusEffectFormatter;
            _actionDefinitionFormatter = actionDefinitionFormatter;
        }

        public string FormatData(FunctionalItemTraitBase data)
        {
            return data switch
            {
                InteractionTrait interactionTrait => _interactionDefinitionFormatter.FormatData(interactionTrait.Interaction),
                StatModifiersTrait statModifiersTrait => _statModifiersFormatter.FormatData(statModifiersTrait.Modifiers),
                StatusEffectTrait statusEffectTrait => _statusEffectFormatter.FormatData(statusEffectTrait.StatusEffect),
                SpecialActionTrait specialActionTrait => FormatSpecialActionTrait(specialActionTrait),
                _ => string.Empty
            };
        }

        private string FormatSpecialActionTrait(SpecialActionTrait trait)
        {
            var kindDescription = trait.Kind.GetDescription();
            var actionText = _actionDefinitionFormatter.FormatData(trait.Action);

            return $"<b><u>{kindDescription}</u></b> Action:{Environment.NewLine}{actionText}";
        }
    }
}
