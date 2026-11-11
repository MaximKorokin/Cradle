using Assets._Game.Scripts.Shared.Unity;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views.Widgets;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class StatsView : UIViewBase<IStatsViewData>
    {
        [SerializeField]
        private RectTransform _statsParent;
        [SerializeField]
        private StatWidget _statTemplate;

        private readonly GameObjectCollection _instantiatedObjects = new();

        protected override void Awake()
        {
            base.Awake();
            _statTemplate.gameObject.SetActive(false);
        }

        protected override void Render(IStatsViewData statsViewData)
        {
            Clear();

            foreach (var (stat, value) in statsViewData.Stats)
            {
                var statView = _instantiatedObjects.Instantiate(_statTemplate, _statsParent);
                statView.Render((stat, value));
            }
        }

        public void Clear()
        {
            _instantiatedObjects.Clear();
        }
    }
}
