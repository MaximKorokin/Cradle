using Assets._Game.Scripts.Entities.Interactions;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class InteractionDefinitionFormatter : IDataFormatter<InteractionDefinition, string>
    {
        public string FormatData(InteractionDefinition data)
        {
            return "Applies interaction";
        }
    }
}
