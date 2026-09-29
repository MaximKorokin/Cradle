using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class QuestGiverWindow : UIWindowBase
    {
        [field: SerializeField]
        public QuestGiverView QuestGiverView { get; private set; }
    }
}
