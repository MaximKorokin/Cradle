using System;

namespace Assets._Game.Scripts.Shared.Attributes
{
    public sealed class EnumDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public EnumDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
