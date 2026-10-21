using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Shared.Extensions;
using System.Text;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class StatusEffectFormatter : IDataFormatter<StatusEffectSnapshot, string>
    {
        private readonly StatModifiersFormatter _statModifiersFormatter;
        private readonly AttackModifiersFormatter _attackModifierFormatter;

        public StatusEffectFormatter(
            StatModifiersFormatter statModifiersFormatter,
            AttackModifiersFormatter attackModifierFormatter)
        {
            _statModifiersFormatter = statModifiersFormatter;
            _attackModifierFormatter = attackModifierFormatter;
        }

        public string FormatData(StatusEffectSnapshot data)
        {
            if (data.Definition == null)
                return string.Empty;

            var definition = data.Definition;

            var stringBuilder = new StringBuilder();

            // Name and category
            stringBuilder.Append($"<b><u>{definition.Name}</u></b> ({definition.Category.GetDescription()})");

            // Lifetime behaviour
            if (definition.Behaviour.HasFlag(StatusEffectBehaviour.Duration))
                stringBuilder.Append($", {definition.Duration:0.##}s");
            if (definition.Behaviour.HasFlag(StatusEffectBehaviour.Charges))
                stringBuilder.Append($", {data.RemainingChargesAmount} charges");
            if (definition.Behaviour.HasFlag(StatusEffectBehaviour.Stacks))
                stringBuilder.Append($", {definition.Stacks} stacks");

            // Stat modifiers
            if (definition.StatModifiers != null && definition.StatModifiers.Length > 0)
            {
                var statModifiersText = _statModifiersFormatter.FormatData(definition.StatModifiers);
                if (!string.IsNullOrEmpty(statModifiersText))
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append(statModifiersText);
                }
            }

            // Attack modifiers
            if (definition.AttackModifiers != null && definition.AttackModifiers.Length > 0)
            {
                var attackModifiersText = _attackModifierFormatter.FormatData(definition.AttackModifiers);
                if (!string.IsNullOrEmpty(attackModifiersText))
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append(attackModifiersText);
                }
            }

            // Control provider (if present)
            if (definition.ControlProvider != null)
            {
                stringBuilder.AppendLine();
                var controlType = definition.ControlProvider.GetType().Name.Replace("ControlProviderData", "");
                stringBuilder.Append($"Control: {controlType}");
            }

            return stringBuilder.ToString();
        }
    }
}
