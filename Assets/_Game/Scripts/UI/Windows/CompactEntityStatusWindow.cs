using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class CompactEntityStatusWindow : UIWindowBase
    {
        [field: SerializeField]
        public CompactEntityStatusView CompactPlayerStateView { get; private set; }
    }
}
