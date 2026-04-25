using Assets._Game.Scripts.Entities.Interactions;
using Assets._Game.Scripts.Shared.Extensions;
using System.Text;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class InteractionDefinitionFormatter : IDataFormatter<InteractionDefinition, string>
    {
        public string FormatData(InteractionDefinition data)
        {
            if (data == null || data.Root == null)
                return string.Empty;

            var stringBuilder = new StringBuilder();
            FormatStep(data.Root, stringBuilder);
            return stringBuilder.ToString().TrimEnd();
        }

        private void FormatStep(InteractionDefinition.StepData step, StringBuilder stringBuilder)
        {
            if (step == null)
                return;

            switch (step)
            {
                case InteractionDefinition.StepsStepData stepsData:
                    foreach (var subStep in stepsData.Steps)
                    {
                        FormatStep(subStep, stringBuilder);
                    }
                    break;

                case InteractionDefinition.DamageData damageData:
                    var damageTypeText = damageData.Spec.Type.GetDescription();
                    var flatText = damageData.Spec.Flat > 0 ? $"{(int)damageData.Spec.Flat}" : "";
                    var scaleText = damageData.Spec.AttackScale > 0 ? $"{damageData.Spec.AttackScale * 100:0.##}% ATK" : "";

                    var damageValue = "";
                    if (damageData.Spec.Flat > 0 && damageData.Spec.AttackScale > 0)
                        damageValue = $"{flatText} + {scaleText}";
                    else if (damageData.Spec.Flat > 0)
                        damageValue = $"{flatText}";
                    else if (damageData.Spec.AttackScale > 0)
                        damageValue = $"{scaleText}";

                    var critText = damageData.Spec.CanCrit ? "(can crit)" : "";
                    stringBuilder.AppendLine($"Damage: {damageValue} {damageTypeText} {critText}");
                    break;

                case InteractionDefinition.HealData healData:
                    stringBuilder.AppendLine($"Heal: {healData.Amount}");
                    break;

                case InteractionDefinition.OverrideControlStepData controlData:
                    var controlType = controlData.ControlProvider?.GetType().Name.Replace("ControlProviderData", "") ?? "None";
                    stringBuilder.AppendLine($"Override Control: {controlType}");
                    break;

                case InteractionDefinition.ReviveStepData:
                    stringBuilder.AppendLine($"Revive");
                    break;

                default:
                    break;
            }
        }
    }
}
