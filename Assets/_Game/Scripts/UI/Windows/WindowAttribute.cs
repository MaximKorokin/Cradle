using System;

namespace Assets._Game.Scripts.UI.Windows
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class WindowAttribute : Attribute
    {
        public WindowId Id { get; }
        public bool IsSingleton { get; set; } = true;
        public bool IsModal { get; set; } = false;
        public bool CanMove { get; set; } = true;

        public WindowAttribute(WindowId id)
        {
            Id = id;
        }
    }
}
