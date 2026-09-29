using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class QuestDescriptionWindow : UIWindowBase
    {
        [field: SerializeField]
        public QuestDescriptionView QuestDescriptionView { get; private set; }
    }
}
