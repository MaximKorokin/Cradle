using Assets._Game.Scripts.UI.DataAggregators;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class EntityAiToggleView : UIViewBase<IEntityAiToggleViewData>
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

        protected override void Render(IEntityAiToggleViewData viewData)
        {
            _toggle.SetIsOnWithoutNotify(viewData.IsAIEnabled);
        }
    }
}
