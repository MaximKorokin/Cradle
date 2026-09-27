using Assets._Game.Scripts.UI.Systems.Click;
using Assets._Game.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Common
{
    [RequireComponent(typeof(Button))]
    public sealed class WindowOpenTrigger : MonoBehaviour, IClickTarget
    {
        [SerializeField] private WindowId _windowId;

        public WindowId WindowId => _windowId;
    }
}
