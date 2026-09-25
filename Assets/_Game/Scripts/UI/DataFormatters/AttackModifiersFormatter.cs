using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Shared.Extensions;
using System.Collections.Generic;
using System.Text;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class AttackModifiersFormatter : IDataFormatter<IReadOnlyList<AttackModifierDefinition>, string>
    {
        public string FormatData(IReadOnlyList<AttackModifierDefinition> data)
        {
            if (data == null || data.Count == 0)
                return string.Empty;

            var stringBuilder = new StringBuilder();
            for (int i = 0; i < data.Count; i++)
            {
                var modifier = data[i];
                var typeDescription = modifier.Type.GetDescription();
                var sign = modifier.Value > 0 ? "+" : "";
                var valueText = $"{sign}{modifier.Value * 100:0.##}%";
                var chanceText = modifier.Chance < 1 ? $" ({modifier.Chance * 100:0.##}% chance)" : "";

                stringBuilder.Append($"{typeDescription}: {valueText}{chanceText}");

                if (i < data.Count - 1)
                    stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }
    }
}
