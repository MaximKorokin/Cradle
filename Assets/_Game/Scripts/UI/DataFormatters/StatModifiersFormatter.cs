using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Shared.Extensions;
using System.Text;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class StatModifiersFormatter : IDataFormatter<StatModifier[], string>
    {
        public string FormatData(StatModifier[] data)
        {
            var stringBuilder = new StringBuilder();
            foreach (var modifier in data)
            {
                var sign = modifier.Value > 0 ? "+" : "";
                switch (modifier.Operation)
                {
                    case StatOperation.Add:
                        stringBuilder.AppendLine($"{modifier.Stat.GetDescription()}: {sign}{modifier.Value:0.##}");
                        break;
                    case StatOperation.Multiply:
                        stringBuilder.AppendLine($"{modifier.Stat.GetDescription()}: {sign}{modifier.Value * 100:0.##}%");
                        break;
                    case StatOperation.Override:
                        stringBuilder.AppendLine($"{modifier.Stat.GetDescription()}: set to {modifier.Value:0.##}");
                        break;
                    case StatOperation.ClampMin:
                        stringBuilder.AppendLine($"{modifier.Stat.GetDescription()}: min to {modifier.Value:0.##}");
                        break;
                    case StatOperation.ClampMax:
                        stringBuilder.AppendLine($"{modifier.Stat.GetDescription()}: max to {modifier.Value:0.##}");
                        break;
                    default:
                        stringBuilder.AppendLine($"\n(Unknown operation)\n");
                        break;
                }
            }
            return stringBuilder.ToString();
        }
    }
}
