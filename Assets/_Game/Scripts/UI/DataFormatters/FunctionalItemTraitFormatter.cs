using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Traits;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class FunctionalItemTraitFormatter : IDataFormatter<FunctionalItemTraitBase, string>
    {
        private readonly InteractionDefinitionFormatter _interactionDefinitionFormatter;
        private readonly StatModifiersFormatter _statModifiersFormatter;
        private readonly StatusEffectFormatter _statusEffectFormatter;

        public FunctionalItemTraitFormatter(
            InteractionDefinitionFormatter interactionDefinitionFormatter,
            StatModifiersFormatter statModifiersFormatter,
            StatusEffectFormatter statusEffectFormatter)
        {
            _interactionDefinitionFormatter = interactionDefinitionFormatter;
            _statModifiersFormatter = statModifiersFormatter;
            _statusEffectFormatter = statusEffectFormatter;
        }

        public string FormatData(FunctionalItemTraitBase data)
        {
            return data switch
            {
                InteractionTrait interactionTrait => _interactionDefinitionFormatter.FormatData(interactionTrait.Interaction),
                StatModifiersTrait statModifiersTrait => _statModifiersFormatter.FormatData(statModifiersTrait.Modifiers),
                StatusEffectTrait statusEffectTrait => _statusEffectFormatter.FormatData(statusEffectTrait.StatusEffect),
                _ => string.Empty
            };
        }
    }
}
