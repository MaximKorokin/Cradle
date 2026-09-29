using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class QuestsWindow : UIWindowBase
    {
        [field: SerializeField]
        public QuestsView QuestsView { get; private set; }
    }
}
