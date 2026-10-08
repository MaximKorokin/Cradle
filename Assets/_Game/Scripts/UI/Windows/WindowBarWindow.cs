using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class WindowBarWindow : UIWindowBase
    {
        [field: SerializeField]
        public EmptyView View { get; private set; }
    }
}
