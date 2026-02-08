using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class StatsView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _statsParent;
        [SerializeField]
        private StatView _statTemplate;

        private readonly List<StatView> _statViews = new();

        public void Render(IEnumerable<(string, string)> stats)
        {
            _statTemplate.gameObject.SetActive(false);

            foreach (var (stat, value) in stats)
            {
                var statView = Instantiate(_statTemplate, _statsParent);
                _statViews.Add(statView);
                statView.Render(stat, value);
            }
        }

        public void Clear()
        {
            foreach (var statView in _statViews)
            {
                Destroy(statView);
            }
            _statViews.Clear();
        }
    }
}
