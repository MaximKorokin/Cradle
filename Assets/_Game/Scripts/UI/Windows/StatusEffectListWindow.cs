using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public class StatusEffectListWindow : UIWindowBase
    {
        [field: SerializeField]
        public StatusEffectListView StatusEffectListView { get; private set; }
    }
}
