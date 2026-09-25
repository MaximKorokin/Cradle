using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class StatsView : UIViewBase<IEnumerable<(string, string)>>
    {
        [SerializeField]
        private RectTransform _statsParent;
        [SerializeField]
        private StatView _statTemplate;

        private readonly List<StatView> _statViews = new();

        protected override void Awake()
        {
            base.Awake();
            _statTemplate.gameObject.SetActive(false);
        }

        public override void Render(IEnumerable<(string, string)> stats)
        {
            Clear();

            foreach (var (stat, value) in stats)
            {
                var statView = Instantiate(_statTemplate, _statsParent);
                _statViews.Add(statView);
                statView.Render((stat, value));
            }
        }

        public void Clear()
        {
            foreach (var statView in _statViews)
            {
                Destroy(statView.gameObject);
            }
            _statViews.Clear();
        }
    }
}
