using Assets._Game.Scripts.UI.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class StatsWindow : UIWindow
    {
        [SerializeField]
        private StatsView _statsView;

        public void Render(IEnumerable<(string, string)> stats)
        {
            _statsView.Render(stats);
        }
    }
}
