using Assets._Game.Scripts.Entities.StatusEffects;
using System;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class StatusEffectFormatter : IDataFormatter<StatusEffectDefinition, string>
    {
        public string FormatData(StatusEffectDefinition data)
        {
            return $"{data.Name} ({Math.Round(data.Duration, 1)}s, {data.Charges} charges)";
        }
    }
}
