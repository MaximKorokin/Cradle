using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Shared.Extensions;
using System;
using System.Text;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class StatModifiersFormatter : IDataFormatter<StatModifier[], string>
    {
        public string FormatData(StatModifier[] data)
        {
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                var modifier = data[i];
                var sign = modifier.Value > 0 ? "+" : "";
                switch (modifier.Operation)
                {
                    case StatOperation.Add:
                        stringBuilder.Append($"{modifier.Stat.GetDescription()}: {sign}{modifier.Value:0.##}");
                        break;
                    case StatOperation.Multiply:
                        stringBuilder.Append($"{modifier.Stat.GetDescription()}: {sign}{modifier.Value * 100:0.##}%");
                        break;
                    case StatOperation.Override:
                        stringBuilder.Append($"{modifier.Stat.GetDescription()}: set to {modifier.Value:0.##}");
                        break;
                    case StatOperation.ClampMin:
                        stringBuilder.Append($"{modifier.Stat.GetDescription()}: min to {modifier.Value:0.##}");
                        break;
                    case StatOperation.ClampMax:
                        stringBuilder.Append($"{modifier.Stat.GetDescription()}: max to {modifier.Value:0.##}");
                        break;
                    default:
                        stringBuilder.Append($"{Environment.NewLine}(Unknown operation){Environment.NewLine}");
                        break;
                }

                if (i < data.Length - 1)
                {
                    stringBuilder.AppendLine();
                }
            }
            return stringBuilder.ToString();
        }
    }
}
