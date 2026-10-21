using Assets._Game.Scripts.UI.DataAggregators;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class StatusEffectListView : UIViewBase<StatusEffectListViewData>
    {
        [field: SerializeField]
        public StatusEffectView BuffTemplate { get; private set; }
        [field: SerializeField]
        public StatusEffectView DebuffTemplate { get; private set; }
        [field: SerializeField]
        public RectTransform BuffsContainer { get; private set; }
        [field: SerializeField]
        public RectTransform DebuffsContainer { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            BuffTemplate.gameObject.SetActive(false);
            DebuffTemplate.gameObject.SetActive(false);
        }

        protected override void Render(StatusEffectListViewData data)
        {
        }
    }
}
