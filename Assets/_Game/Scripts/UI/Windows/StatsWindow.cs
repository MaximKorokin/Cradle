using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class StatsWindow : UIWindowBase
    {
        [field: SerializeField]
        public StatsView StatsView { get; private set; }
    }
}
