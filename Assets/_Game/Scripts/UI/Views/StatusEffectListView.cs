using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views.Widgets;
using System.Diagnostics;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class StatusEffectListView : UIViewBase<StatusEffectListViewData>
    {
        [field: SerializeField]
        public StatusEffectWidget BuffTemplate { get; private set; }
        [field: SerializeField]
        public StatusEffectWidget DebuffTemplate { get; private set; }
        [SerializeField]
        private Transform _buffsContainer;
        [SerializeField]
        private Transform _debuffsContainer;

        protected override void Awake()
        {
            base.Awake();

            BuffTemplate.gameObject.SetActive(false);
            DebuffTemplate.gameObject.SetActive(false);
        }

        protected override void Render(StatusEffectListViewData data)
        {
            Clear();
            foreach (var buff in data.Buffs)
            {
                CreateAndRenderWidget(BuffTemplate, _buffsContainer, buff);
            }
            foreach (var debuff in data.Debuffs)
            {
                CreateAndRenderWidget(DebuffTemplate, _debuffsContainer, debuff);
            }
        }

        private StatusEffectWidget CreateAndRenderWidget(StatusEffectWidget template, Transform parent, StatusEffectSnapshot statusEffect)
        {
            var widget = Instantiate(template, parent);
            widget.gameObject.SetActive(true);
            widget.Render(statusEffect);
            return widget;
        }

        private void Clear()
        {
            foreach (Transform child in _buffsContainer)
            {
                if (child == BuffTemplate.transform) continue;
                Destroy(child.gameObject);
            }
            foreach (Transform child in _debuffsContainer)
            {
                if (child == DebuffTemplate.transform) continue;
                Destroy(child.gameObject);
            }
        }
    }
}
