using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Widgets;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class CheatsWindow : UIWindowBase
    {
        [field: SerializeField]
        public GameControlWidget GameControlTabContent { get; private set; }

        [field: SerializeField]
        public CheatsView CheatsView { get; private set; }
    }
}

