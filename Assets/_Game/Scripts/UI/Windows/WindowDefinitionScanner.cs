using Assets._Game.Scripts.UI.Windows.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.Windows
{
    public static class WindowDefinitionScanner
    {
        /// <summary> Builds window definitions from all concrete <see cref="IWindowController"/> types marked with <see cref="WindowAttribute"/>. </summary>
        public static IReadOnlyList<WindowDefinition> Scan()
        {
            var definitions = new List<WindowDefinition>();
            var controllerTypes = typeof(WindowDefinitionScanner).Assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition && typeof(IWindowController).IsAssignableFrom(t));

            foreach (var type in controllerTypes)
            {
                var attribute = (WindowAttribute)Attribute.GetCustomAttribute(type, typeof(WindowAttribute), false);
                if (attribute == null)
                {
                    SLog.Warn($"Window controller {type.Name} has no [Window] attribute and will not be registered.");
                    continue;
                }

                if (attribute.Id == WindowId.None)
                {
                    throw new InvalidOperationException($"Window controller {type.Name} uses {nameof(WindowId.None)} as its window id.");
                }

                var duplicate = definitions.FirstOrDefault(d => d.Id == attribute.Id);
                if (duplicate != null)
                {
                    throw new InvalidOperationException($"Window {attribute.Id} is declared by both {duplicate.ControllerType.Name} and {type.Name}.");
                }

                definitions.Add(new WindowDefinition(
                    attribute.Id,
                    type,
                    new WindowConfiguration(attribute.IsSingleton, attribute.IsModal, attribute.CanMove)));
            }

            return definitions;
        }
    }
}
