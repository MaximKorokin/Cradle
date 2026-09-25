using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class PlayerAiToggleView : UIViewBase<bool>
    {
        [SerializeField]
        private Toggle _toggle;

        public event Action<bool> ValueChanged;

        protected override void Awake()
        {
            base.Awake();
            _toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _toggle.onValueChanged.RemoveListener(OnToggleChanged);
        }

        private void OnToggleChanged(bool value)
        {
            ValueChanged?.Invoke(value);
        }

        public override void Render(bool data)
        {

        }
    }
}
