using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class CraftingWindow : UIWindowBase
    {
        [field: SerializeField]
        public CraftingView CraftingView { get; private set; }
    }
}
