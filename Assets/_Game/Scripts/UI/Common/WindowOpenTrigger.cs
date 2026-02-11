using Assets._Game.Scripts.UI.Windows;
using Assets.CoreScripts;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Assets._Game.Scripts.UI.Common
{
    public sealed class WindowOpenTrigger : MonoBehaviour
    {
        [SerializeField] private WindowId _windowId;

        private WindowManager _windowManager;

        [Inject]
        private void Construct(WindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                _windowManager.InstantiateWindow(_windowId);
            });
        }
    }
}
