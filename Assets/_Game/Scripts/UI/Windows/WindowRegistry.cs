using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.UI.Windows.Controllers;
using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.UI.Windows
{
    /// <summary> Holds window definitions and prefabs and validates that they match each other. </summary>
    public sealed class WindowRegistry
    {
        private readonly Dictionary<WindowId, WindowDefinition> _definitions = new();
        private readonly Dictionary<Type, UIWindowBase> _prefabsByType = new();

        public WindowRegistry(IEnumerable<WindowDefinition> windowDefinitions, UIWindowsConfig windowPrefabsConfig)
        {
            if (windowPrefabsConfig == null || windowPrefabsConfig.WindowPrefabs == null)
            {
                throw new InvalidOperationException("Window prefab configuration is not registered or contains no prefab list.");
            }

            foreach (var prefab in windowPrefabsConfig.WindowPrefabs)
            {
                if (prefab == null)
                {
                    throw new InvalidOperationException("Window prefab configuration contains a null entry.");
                }

                var prefabType = prefab.GetType();
                if (_prefabsByType.ContainsKey(prefabType))
                {
                    throw new InvalidOperationException($"More than one window prefab is registered for type {prefabType}.");
                }

                _prefabsByType.Add(prefabType, prefab);
            }

            foreach (var definition in windowDefinitions)
            {
                if (definition == null)
                {
                    throw new InvalidOperationException("Window definitions contain a null entry.");
                }

                if (_definitions.ContainsKey(definition.Id))
                {
                    throw new InvalidOperationException($"More than one definition is registered for window {definition.Id}.");
                }

                _definitions.Add(definition.Id, definition);
            }

            Validate();
        }

        public WindowDefinition GetDefinition(WindowId windowId)
        {
            if (!_definitions.TryGetValue(windowId, out var definition))
            {
                throw new InvalidOperationException($"No definition for window {windowId} registered.");
            }

            return definition;
        }

        public UIWindowBase GetPrefab(Type windowType)
        {
            if (!_prefabsByType.TryGetValue(windowType, out var prefab))
            {
                throw new InvalidOperationException($"No prefab for window type {windowType} is registered.");
            }

            return prefab;
        }

        private void Validate()
        {
            var usedWindowTypes = new HashSet<Type>();
            foreach (var definition in _definitions.Values)
            {
                var windowType = GetWindowType(definition.ControllerType);
                if (windowType == null)
                {
                    throw new InvalidOperationException($"Controller {definition.ControllerType.Name} of window {definition.Id} does not derive from {typeof(WindowControllerBase<,>).Name}.");
                }

                if (!_prefabsByType.ContainsKey(windowType))
                {
                    throw new InvalidOperationException($"Window {definition.Id} requires a prefab of type {windowType.Name}, but none is registered in the window prefabs config.");
                }

                usedWindowTypes.Add(windowType);
            }

            foreach (var prefabType in _prefabsByType.Keys)
            {
                if (!usedWindowTypes.Contains(prefabType))
                {
                    SLog.Warn($"Window prefab of type {prefabType.Name} has no controller with a [Window] attribute.");
                }
            }
        }

        private static Type GetWindowType(Type controllerType)
        {
            for (var type = controllerType; type != null; type = type.BaseType)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(WindowControllerBase<,>))
                {
                    return type.GetGenericArguments()[0];
                }
            }
            return null;
        }
    }
}
