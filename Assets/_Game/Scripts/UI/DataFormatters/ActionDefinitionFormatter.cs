using Assets._Game.Scripts.Entities.Interactions.Action;
using Assets._Game.Scripts.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class ActionDefinitionFormatter : IDataFormatter<ActionDefinition, string>
    {
        private readonly InteractionDefinitionFormatter _interactionDefinitionFormatter;

        private readonly List<string> _tags = new();

        public ActionDefinitionFormatter(InteractionDefinitionFormatter interactionDefinitionFormatter)
        {
            _interactionDefinitionFormatter = interactionDefinitionFormatter;
        }

        public string FormatData(ActionDefinition data)
        {
            if (data == null)
                return string.Empty;

            _tags.Clear();
            var stringBuilder = new StringBuilder();

            // Name
            stringBuilder.Append($"<b><u>{data.Name}</u></b>");

            // Cost and resource info
            if (data.ManaCost > 0)
                _tags.Add($"{data.ManaCost:0.##} mana");
            if (data.ConsumesCharges)
                _tags.Add("Uses SE charges");

            // Timing info
            if (data.PreparationTime > 0)
                _tags.Add($"Preparation: {data.PreparationTime:0.##}s");
            //if (data.MaxChannelingTime > 0)
            //    _tags.Add($"Max channeling: {data.MaxChannelingTime:0.##}s");
            if (data.Cooldown > 0)
                _tags.Add($"Cooldown: {data.Cooldown:0.##}s");

            // Range and speed
            if (data.Range > 0)
                _tags.Add($"Range: {data.Range:0.##}");
            if (data.SpeedMultiplier != ActionExecutionSpeedMultiplier.None)
                _tags.Add($"Speed mod.: {data.SpeedMultiplier.GetDescription()}");

            if (_tags.Count > 0)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append($"<size=90%>{string.Join(Environment.NewLine, _tags.Select(tag => $"[<i>{tag}</i>]"))}</size>");
                //stringBuilder.Append($"<size=90%><i>{string.Join(Environment.NewLine, _tags)}</i></size>");
            }

            // Interaction effects
            if (data.Interaction != null)
            {
                stringBuilder.AppendLine();
                var interactionText = _interactionDefinitionFormatter.FormatData(data.Interaction);
                if (!string.IsNullOrEmpty(interactionText))
                    stringBuilder.Append(interactionText);
            }

            // Description (if provided)
            if (!string.IsNullOrEmpty(data.Description))
            {
                stringBuilder.AppendLine();
                stringBuilder.Append($"<i>--- {data.Description}</i>");
            }

            return stringBuilder.ToString();
        }
    }
}
