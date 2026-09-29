using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class LocationTransitionListWindow : UIWindowBase
    {
        [field: SerializeField]
        public LocationTransitionListView LocationTransitionListView { get; private set; }
    }
}
