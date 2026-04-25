using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Shared.Extensions;
using System.Text;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class StatusEffectFormatter : IDataFormatter<StatusEffectDefinition, string>
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

        public string FormatData(StatusEffectDefinition data)
        {
            if (data == null)
                return string.Empty;

            var stringBuilder = new StringBuilder();

            // Name and category
            stringBuilder.Append($"<b><u>{data.Name}</u></b> ({data.Category.GetDescription()})");

            // Lifetime behaviour
            if (data.Behaviour.HasFlag(StatusEffectBehaviour.Duration))
                stringBuilder.Append($", {data.Duration:0.##}s");
            if (data.Behaviour.HasFlag(StatusEffectBehaviour.Charges))
                stringBuilder.Append($", {data.Charges} charges");
            if (data.Behaviour.HasFlag(StatusEffectBehaviour.Stacks))
                stringBuilder.Append($", {data.Stacks} stacks");

            // Stat modifiers
            if (data.StatModifiers != null && data.StatModifiers.Length > 0)
            {
                var statModifiersText = _statModifiersFormatter.FormatData(data.StatModifiers);
                if (!string.IsNullOrEmpty(statModifiersText))
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append(statModifiersText);
                }
            }

            // Attack modifiers
            if (data.AttackModifiers != null && data.AttackModifiers.Length > 0)
            {
                var attackModifiersText = _attackModifierFormatter.FormatData(data.AttackModifiers);
                if (!string.IsNullOrEmpty(attackModifiersText))
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append(attackModifiersText);
                }
            }

            // Control provider (if present)
            if (data.ControlProvider != null)
            {
                stringBuilder.AppendLine();
                var controlType = data.ControlProvider.GetType().Name.Replace("ControlProviderData", "");
                stringBuilder.Append($"Control: {controlType}");
            }

            return stringBuilder.ToString();
        }
    }
}
