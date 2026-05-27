using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class PlayerAiToggleView : MonoBehaviour
    {
        [SerializeField]
        private Toggle _toggle;

        public event Action<bool> ValueChanged;

        private void Awake()
        {
            _toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        private void OnDestroy()
        {
            _toggle.onValueChanged.RemoveListener(OnToggleChanged);
        }

        public void SetValueWithoutNotify(bool value)
        {
            _toggle.SetIsOnWithoutNotify(value);
        }

        private void OnToggleChanged(bool value)
        {
            ValueChanged?.Invoke(value);
        }
    }
}
