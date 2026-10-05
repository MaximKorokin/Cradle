using System;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class InitialWindowsDataProvider : MonoBehaviour
    {
        [SerializeField]
        private WindowInitializerData[] _windowInitializers;

        public WindowInitialData[] GetWindowsInitialData()
        {
            var windowInitialStates = new WindowInitialData[_windowInitializers.Length];
            for (int i = 0; i < _windowInitializers.Length; i++)
            {
                var initializer = _windowInitializers[i];
                windowInitialStates[i] = new WindowInitialData(
                    initializer.WindowId,
                    initializer.WindowSettings,
                    initializer.TransformPosition.position
                );
            }
            return windowInitialStates;
        }

        [Serializable]
        private class WindowInitializerData
        {
            [field: SerializeField]
            public WindowId WindowId { get; private set; }
            [field: SerializeField]
            public WindowSettings WindowSettings { get; private set; }
            [field: SerializeField]
            public RectTransform TransformPosition { get; private set; }
        }
    }

    public readonly struct WindowInitialData
    {
        public readonly WindowId WindowId;
        public readonly WindowSettings WindowSettings;
        public readonly Vector2 Position;

        public WindowInitialData(WindowId windowId, WindowSettings windowSettings, Vector3 position)
        {
            WindowId = windowId;
            WindowSettings = windowSettings;
            Position = position;
        }
    }
}
